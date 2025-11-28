using Enums;
using Options;
using Models;

namespace Business_Layer.Interfaces;

public interface IUsersBusiness
{ 
    int GetUserId();
    Task<User> Login(AuthenticationRequest data);
    Task<User> InsertUser(string name, string email, string password);
    Task<string> IsValidRegisterRequest(RegisterRequest request);

}
