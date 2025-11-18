using Models;

namespace Data_Layer.Interfaces;

public interface IUsersData
{ 
    Task<User> GetUserByEmail(string email);
    Task<User> InsertUser(string name, string email, string password);
}
