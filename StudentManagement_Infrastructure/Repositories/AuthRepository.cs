using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StudentManagement_Domain.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using StudentManagement_Application.Services;
using Microsoft.Extensions.Configuration;
namespace StudentManagement_Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthRepository(UserManager<IdentityUser> userManager, EmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            if (user.TwoFactorEnabled)
            {
                var message = await SendOtpAfterLogin(email);
                return message;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims != null && userClaims.Any())
            {
                claims.AddRange(userClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CJKFOGk-9E0aI8Gv09mD-8utzSyLQx_yrJKi1fXc6Y7CeYszLzcmMA2C0_Ej3K7BQdsCW9zoqW3a-5L1ZNRytFC0BeA6dZLsCjoTrFoI9guwvEmJ0gbN9yHQ0fDYbkwGUyJbP6eNEzKbWHMarSx7RWGKaGsxy0qguEMSO3OUWU8"));
            var tokenDescriptor = new JwtSecurityToken(
                issuer: "localhost",
                audience: "audience1",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;
        }

        public async Task<(bool Succeeded, string[] Errors, IdentityUser? User)> RegisterAsync(string email, string password)
        {
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false, // Chưa xác nhận email
                TwoFactorEnabled = true // Bật xác thực hai bước
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return (false, result.Errors.Select(e => e.Description).ToArray(), null);
            }

            // create otp
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // send email
            //var callbackUrl = $"https://localhost:44378/api/Auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            //await _emailService.SendEmailAsync(
            //    email,
            //    "Confirm your email",
            //    $"<p>Please confirm your account by clicking the link below:</p><a href='{callbackUrl}'>Confirm Email</a>"
            //);

            return (true, Array.Empty<string>(), user);
        }

        public async Task<IdentityResult> AddClaimToUserAsync(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            return result;
        }

        public async Task<string> ValidateOtpForLogin(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ("User not found.");
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, otp);
            if (!isValid)
            {
                return ("Invalid OTP.");
            }

            //await _userManager.SignInAsync(user, isPersistent: false);

            // Generate JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims != null && userClaims.Any())
            {
                claims.AddRange(userClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CJKFOGk-9E0aI8Gv09mD-8utzSyLQx_yrJKi1fXc6Y7CeYszLzcmMA2C0_Ej3K7BQdsCW9zoqW3a-5L1ZNRytFC0BeA6dZLsCjoTrFoI9guwvEmJ0gbN9yHQ0fDYbkwGUyJbP6eNEzKbWHMarSx7RWGKaGsxy0qguEMSO3OUWU8"));
            var tokenDescriptor = new JwtSecurityToken(
                issuer: "localhost",
                audience: "audience1",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;
        }

        public async Task<string> SendOtpAfterLogin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            if (user.TwoFactorEnabled)
            {
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Your OTP Code",
                    $"<p>Xin chào {user.UserName},</p>" +
                    $"<p>Bạn đã yêu cầu xác thực OTP. Vui lòng click vào link dưới đây để xác thực:</p>" +
                    $"<p><a href='{"http://localhost:3000/students"}'>Xác thực OTP</a></p>" +
                    $"<p>Mã OTP: <strong>{otp}</strong></p>"
                );

                return "OTP has been sent to your email.";
            }

            throw new InvalidOperationException("Two-factor authentication is not enabled for this user.");
        }

        public async Task<(IList<string> Roles, IList<Claim> Claims)> GetUserRolesAndClaimsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            return (roles, claims);
        }
    }
}
