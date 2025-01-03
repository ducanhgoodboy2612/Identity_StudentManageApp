using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.Interface
{
    public partial interface IAuthRepository
    {
        Task<string> AuthenticateUserAsync(string email, string password);
        Task<IdentityResult> AddClaimToUserAsync(string userId, string claimType, string claimValue);
        Task<(bool Succeeded, string[] Errors, IdentityUser? User)> RegisterAsync(string email, string password);
        Task<(IList<string> Roles, IList<Claim> Claims)> GetUserRolesAndClaimsAsync(string email);
        Task<string> ValidateOtpForLogin(string email, string otp);
    }
}
