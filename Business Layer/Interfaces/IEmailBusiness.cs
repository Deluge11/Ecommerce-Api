

namespace Business_Layer.Interfaces;

public interface IEmailBusiness
{
    Task<bool> EmailExists(string email);
}
