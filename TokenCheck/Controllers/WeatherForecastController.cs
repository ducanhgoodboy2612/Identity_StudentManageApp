using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagement_API;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagement_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetToken")]
        public string CreateAccessToken()
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Tom"),
                    new Claim(ClaimTypes.Email, "Tom@gmail.com")
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CJKFOGk-9E0aI8Gv09mD-8utzSyLQx_yrJKi1fXc6Y7CeYszLzcmMA2C0_Ej3K7BQdsCW9zoqW3a-5L1ZNRytFC0BeA6dZLsCjoTrFoI9guwvEmJ0gbN9yHQ0fDYbkwGUyJbP6eNEzKbWHMarSx7RWGKaGsxy0qguEMSO3OUWU8"));
            var jwtInfo = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "audience1",
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(4)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );
            var Token = new JwtSecurityTokenHandler().WriteToken(jwtInfo);
            return Token;
        }

        [HttpGet("api")]
        [Authorize]
        public string Test()
        {
            return "You have pass the bearer";
        }

        [HttpPost("addRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Email and role cannot be empty.");
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the role exists
            var roleExists = await _userManager.IsInRoleAsync(user, role);
            if (roleExists)
            {
                return BadRequest("User already has this role.");
            }

            // Add the role to the user
            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                return Ok($"Role '{role}' has been added to user '{email}'.");
            }

            // Return errors if adding the role failed
            return BadRequest(result.Errors);
        }

    }
}
