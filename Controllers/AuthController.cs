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
        private readonly List<User> _users = new List<User>
        {
            new User
            {
                ID = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test",
                Roles = new List<string>(){
                    "Create",
                    "Update",
                    "Admin",
                    "Delete"
                }
            },
            new User
            {
                ID = 2, FirstName = "Test2", LastName = "User2", Username = "test2", Password = "test2",
                Roles = new List < string >() 
                {
                    "Create",
                    "Get"
                   
                }
            },
            new User
            {
                ID = 3, FirstName = "Test3", LastName = "User3", Username = "test3", Password = "test3",
                Roles = new List < string >() 
                { 
                    "Delete"
                }
            }
        };

        [HttpPost]
        public ActionResult<LoginResultModel> Login(LoginModel model)
        {
            var user = _users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            
            if (user == null)
            {
                return NotFound(new
                {
                    message = "Username or password is not correct"
                });
            }
            _context.User.Add(user);
            _context.SaveChanges();
            var token = GenerateJwtToken(user);
            return Ok(new LoginResultModel
            {
                UserId = user.ID,
                AuthToken = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SigningKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
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
