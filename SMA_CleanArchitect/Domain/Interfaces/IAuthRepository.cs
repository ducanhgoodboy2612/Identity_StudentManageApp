using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Interfaces
{
    public partial interface IAuthRepository
    {
        //Task<IdentityUser> RegisterAsync(string email, string password);
        //Task<(bool Succeeded, string[] Errors, IdentityUser? User)> RegisterAsync(string email, string password);
        //Task<string> LoginAsync(string email, string password);
        //Task<string> ForgotPasswordAsync(string email);
        //Task ResetPasswordAsync(string email, string resetToken, string newPassword);
        //IActionResult GetGoogleLoginProperties(string returnUrl);
        //Task<IdentityResult> ProcessGoogleCallbackAsync(string returnUrl);
        //Task<(bool Success, string Message, IEnumerable<IdentityError> Errors)> ConfirmEmailAsync(string userId, string token);
        //Task<string> ValidateOtpForLogin(string email, string otp);
        //Task AddClaimsToUser(string userId);


        Task<string> SignIn(string email, string password);
    }
}
