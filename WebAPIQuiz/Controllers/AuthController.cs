using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebAPIQuiz.Models;
using WebAPIQuiz.Utils;

namespace WebAPIQuiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
        //Hardcoded Credentials
        private readonly List<(string Username, string Password, string Role)> users = new()
        {
            ("admin", "1234", "Admin"),
            ("user1", "password1", "User"),
            ("user2", "password2", "User")
        };

        [HttpPost("Login")]
        [APIKeyAuthorize]
        public IActionResult Login([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] LoginModel model)
        {
            var user = users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user != default)
            {
                var token = _jwtService.GenerateToken(user.Username, user.Role);

                return Ok(new
                {
                    token,
                    username = user.Username,
                    role = user.Role
                });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
        [HttpPost("refresh")]
        [APIKeyAuthorize]
        public IActionResult RefreshToken([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] LoginModel model)
        {
            var user = users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user != default)
            {
                var token = _jwtService.GenerateToken(user.Username, user.Role);
                return Ok(new
                {
                    token,
                    username = user.Username,
                    role = user.Role
                });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
    }
}
