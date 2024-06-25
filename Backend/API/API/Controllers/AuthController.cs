using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Server.Data;
using API.Server.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace API.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var keyString = _configuration["Jwt:Key"];
            byte[] key;
            try
            {
                key = Convert.FromBase64String(keyString);
            }
            catch (FormatException ex)
            {
                // Xử lý lỗi khi Jwt:Key không phải là chuỗi Base64 hợp lệ
                return BadRequest("Invalid Jwt:Key configuration. Please provide a valid Base64 string.");
            }

            var user = _context.Users.SingleOrDefault(u => u.UserName == login.UserName && u.PasswordHash == login.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
                    // new Claim("IsAdmin", user.i.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

    }
}