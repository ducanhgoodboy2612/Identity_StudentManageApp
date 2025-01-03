using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StudentManagement_Domain.Entities;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.Interface;
namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            var token = await _authRepository.AuthenticateUserAsync(request.Email, request.Password);
            if (token == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Trả về kết quả
            return Ok(new
            {
                Message = "Login successful",
                Token = token
            });
        }

        [HttpPost("addClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(string userId, string claimType, string claimValue)
        {
            if (userId == null || claimType == null || claimValue == null)
            {
                return BadRequest("Invalid input");
            }

            var result = await _authRepository.AddClaimToUserAsync(userId, claimType, claimValue);

            if (result.Succeeded)
            {
                return Ok("Claim added successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (succeeded, errors, user) = await _authRepository.RegisterAsync(model.Email, model.Password);

            if (!succeeded)
            {
                return BadRequest(new
                {
                    Message = "User registration failed.",
                    Errors = errors
                });
            }

            return Ok(new
            {
                Message = "User registered successfully.",
                User = new { user.Email }
            });
        }

        [HttpGet("getUserRolesAndClaims")]
        public async Task<IActionResult> GetUserRolesAndClaims(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");

            try
            {
                var (roles, claims) = await _authRepository.GetUserRolesAndClaimsAsync(email);

                return Ok(new
                {
                    Email = email,
                    Roles = roles,
                    Claims = claims.Select(c => new { c.Type, c.Value })
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otp))
            {
                return BadRequest(new { Message = "Email and OTP are required." });
            }

            var token = await _authRepository.ValidateOtpForLogin(email, otp);
            if (token == null)
            {
                return BadRequest(new { Message = "Error" });
            }

            return Ok(new { Token = token });
        }

        //    [HttpPost("login")]
        //    public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //    {
        //        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        //        {
        //            return BadRequest("Email and Password are required.");
        //        }

        //        // Tìm user trong Identity
        //        var user = await _userManager.FindByEmailAsync(request.Email);
        //        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        //        {
        //            return Unauthorized("Invalid email or password.");
        //        }

        //        // Tạo danh sách claim
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Email, user.Email),
        //    new Claim(ClaimTypes.NameIdentifier, user.Id)
        //};

        //        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CJKFOGk-9E0aI8Gv09mD-8utzSyLQx_yrJKi1fXc6Y7CeYszLzcmMA2C0_Ej3K7BQdsCW9zoqW3a-5L1ZNRytFC0BeA6dZLsCjoTrFoI9guwvEmJ0gbN9yHQ0fDYbkwGUyJbP6eNEzKbWHMarSx7RWGKaGsxy0qguEMSO3OUWU8"));
        //        var tokenDescriptor = new JwtSecurityToken(
        //            issuer: "localhost",
        //            audience: "audience1",
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddMinutes(4),
        //            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        //        );

        //        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        //        return Ok(new
        //        {
        //            Message = "Login successful",
        //            User = new
        //            {
        //                user.Id,
        //                user.UserName,
        //                user.Email,
        //                token
        //            }
        //        });
        //    }


    }
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
