using Microsoft.Extensions.Configuration;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace StudentManagement_Application.Services
{
    public class UserService
    {
        //private readonly IUserRepository _res;
        //private readonly string _secret;

        //public UserService(IUserRepository res, IConfiguration configuration)
        //{
        //    _res = res;
        //    _secret = configuration["AppSettings:Secret"];
        //}

        //public User Login(string username, string password)
        //{
        //    var user = _res.Login(username, password);
        //    if (user == null)
        //        return null;

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, user.Name),
        //            new Claim(ClaimTypes.Email, user.Email)
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    user.Token = tokenHandler.WriteToken(token);

        //    return user;
        //}
    }
}
