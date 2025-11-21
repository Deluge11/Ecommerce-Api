using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business_Layer.Interfaces;
using Options;
using Presentation_Layer.Authorization;
using Models;

namespace Presentation_Layer.Authentication;

public class AuthenticateHelper
{
    public JwtOptions JwtOptions { get; }
    public AuthorizeHelper AuthorizeHelper { get; }
    public IEmailBusiness EmailBusiness { get; }
    public IAuthorizeBusiness AuthorizeBusiness { get; }

    public AuthenticateHelper(
        JwtOptions jwtOptions,
        IEmailBusiness emailBusiness,
        IAuthorizeBusiness authorizeBusiness
        )
    {
        JwtOptions = jwtOptions;
        EmailBusiness = emailBusiness;
        AuthorizeBusiness = authorizeBusiness;
    }


    public async Task<string> CreateToken(User user)
    {
        var claims = new List<Claim>
         {
             new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
             new Claim(ClaimTypes.Name, user.name),
         };


        foreach (var permission in await AuthorizeBusiness.GetPermissions(user.id))
        {
            claims.Add(new Claim("permission", permission.ToString()));
        }


        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = JwtOptions.Issuer,
            Audience = JwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(JwtOptions.Lifetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }


    public async Task<string> IsValidAuthenticate(RegisterRequest request)
    {
        if (request.password.Length < 8)
        {
            return "Password should have 8 letters atleast";
        }
        if (request.password != request.confirmPassword)
        {
            return "Invalid confirm password";
        }
        if (await EmailBusiness.EmailExists(request.email))
        {
            return "Something went wrong";
        }
        return string.Empty;
    }

}
