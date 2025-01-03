using Microsoft.AspNetCore.Identity;
using StudentManagement_Domain.Interface;

namespace StudentManagement_Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public ClaimRepository(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddClaimToUserAsync(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var claim = new System.Security.Claims.Claim(claimType, claimValue);
            var result = await _userManager.AddClaimAsync(user, claim);

            return result;
        }

        public async Task<IdentityResult> AddClaimToRoleAsync(string roleName, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
            }

            var claim = new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            var result = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claimType, claimValue));

            return result;
        }

        public async Task<List<IdentityRoleClaim<string>>> GetClaimsForRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return null;  // Role not found
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var identityRoleClaims = claims.Select(c => new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = c.Type,
                ClaimValue = c.Value
            }).ToList();

            return identityRoleClaims;
        }


        public async Task<IdentityResult> RemoveClaimFromRoleAsync(string roleName, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
            }

            var roleClaims = await _roleManager.GetClaimsAsync(role);

            var claimToRemove = roleClaims.FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);
            if (claimToRemove == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Claim not found" });
            }

            var result = await _roleManager.RemoveClaimAsync(role, claimToRemove);
            return result;
        }

    }
}
