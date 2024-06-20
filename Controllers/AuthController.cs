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

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly List<User> _users = new List<User>
        {
            new User
            {
                Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test",
                Role = "Admin"
            },
            new User
            {
                Id = 2, FirstName = "Test2", LastName = "User2", Username = "test2", Password = "test2",
                Role = "User"
            },
            new User
            {
                Id = 3, FirstName = "Test3", LastName = "User3", Username = "test3", Password = "test3",
                Role = "User2"
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

            var token = GenerateJwtToken(user);
            return Ok(new LoginResultModel
            {
                UserId = user.Id,
                AuthToken = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SigningKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role )
            };

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

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
