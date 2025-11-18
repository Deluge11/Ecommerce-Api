
using Data_Layer.Interfaces;
using Business_Layer.Interfaces;
using Options;
using Business_Layer.Sanitizations;
using System.Security.Claims;
using Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace Business_Layer.Business;

public class UsersBusiness : IUsersBusiness
{
    private IHttpContextAccessor _httpContextAccessor { get; }
    private IUsersData UsersData { get; }
    private HttpContext HttpContext => _httpContextAccessor.HttpContext;

    public UsersBusiness(IHttpContextAccessor httpContext, IUsersData usersData)
    {
        _httpContextAccessor = httpContext;
        UsersData = usersData;
    }


    public int GetUserId()
    {
        var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;
        return int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
    public async Task<User> Login(AuthenticationRequest request)
    {
        var user = await UsersData.GetUserByEmail(request.email);

        if (!IsSamePassword(request.password, user.password))
        {
            return null;
        }

        return user;
    }
    public async Task<User> InsertUser(string name, string email, string password)
    {
        name = Sanitization.SanitizeInput(name);
        email = Sanitization.SanitizeInput(email);
        password = Sanitization.SanitizeInput(password);

        var passwordHasher = new PasswordHasher<object>();
        string hashedPassword = passwordHasher.HashPassword(null, password);

        return await UsersData.InsertUser(name, email, password);
    }
    private bool IsSamePassword(string password, string hashedPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var isValidPassword = passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
        return isValidPassword == PasswordVerificationResult.Success;
    }

}
