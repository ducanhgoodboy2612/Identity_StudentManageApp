using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using Microsoft.Extensions.Configuration;
using Application.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly IConfiguration _configuration;
        private readonly JwtSetting _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthRepository(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            //IConfiguration configuration,
            IOptions<JwtSetting> configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration.Value;
            _roleManager = roleManager;
        }

       

        public async Task<string> SignIn(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return string.Empty;
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                authClaims.Add(claim);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.FindByNameAsync(role));
                foreach (var claim in roleClaims)
                {
                    authClaims.Add(claim);
                }

                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.IssuerSigningKey));

            var token = new JwtSecurityToken(
                issuer: _configuration.ValidIssuer,
                audience: _configuration.ValidAudience,
                expires: DateTime.UtcNow.AddMinutes(20),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> SignUp(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                #region Authorize
                //if (!await _roleManager.RoleExistsAsync(MyRole.Admin))
                //{
                //    var role = new ApplicationRole(MyRole.Admin);
                //    await _roleManager.CreateAsync(role);

                //    var permissions = new List<string>
                //    {
                //        "CanCreateUsers",
                //        "CanManageRoles",
                //        "CanAccessReports"
                //    };

                //    foreach (var permission in permissions)
                //    {
                //        await _roleManager.AddClaimAsync(role, new Claim(MyRole.Admin, permission));
                //    }
                //}

                //await _userManager.AddToRoleAsync(user, MyRole.Admin);
                #endregion

                return true;
            }
            return false;
        }

        public async Task<List<object>> GetAll(int currentPage, int sizePage)
        {
            var query = _userManager.Users;

            int total = await query.CountAsync();
            var countPage = (int)Math.Ceiling((double)total / sizePage);

            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPage) currentPage = countPage;

            if (total == 0 || currentPage == 0)
            {
                return new List<object>();
            }

            var listUser = await query.Skip((currentPage - 1) * sizePage)
                                      .Take(sizePage)
                                      .Select(u => new { u.Id, u.UserName })
                                      .ToListAsync();

            var result = new List<object>();

            foreach (var user in listUser)
            {
                var u = new IdentityUser { Id = user.Id };
                var roles = await _userManager.GetRolesAsync(u);
                var userClaims = await _userManager.GetClaimsAsync(u);

                var roleClaims = new List<object>();
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var claims = await _roleManager.GetClaimsAsync(role);
                        roleClaims.AddRange(claims.Select(c => new { c.Type, c.Value }));
                    }
                }

                result.Add(new
                {
                    user.Id,
                    user.UserName,
                    Roles = roles,
                    UserClaims = userClaims.Select(c => new { c.Type, c.Value }).ToList(),
                    RoleClaims = roleClaims
                });
            }

            return result;
        }

        public async Task<bool> AssignClaim(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (!(await _userManager.GetClaimsAsync(user)).Any(c => c.Type == claimType && c.Value == claimValue))
                    {
                        var newClaim = new Claim(claimType, claimValue);
                        var result = await _userManager.AddClaimAsync(user, newClaim);
                        if (result.Succeeded)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
