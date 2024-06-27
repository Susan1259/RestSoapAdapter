using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace MyApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SampleDBContext _context;

        public AuthController(IConfiguration configuration, SampleDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        

        [HttpPost]
        public ActionResult<LoginResultModel> Login(LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Username or password is not correct"
                });
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            var token = GenerateJwtToken(user);
            return Ok(new LoginResultModel
            {
                UserId = user.UserId,
                AuthToken = token
            });
        }
        [HttpPost("register")]
        public ActionResult Register(UserRegisterModel model)
        {
            // Check if username is already taken
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                return BadRequest(new { message = "Username is already taken" });
            }

            // Create new user entity
            var newUser = new User
            {
                Username = model.Username,
                Password = model.Password,
                UserRoles = model.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList()
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SigningKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),

            };
            claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.RoleName)));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
