
using Business_Layer.Interfaces;
using Data_Layer.Interfaces;


namespace Business_Layer.Business;

public class EmailBusiness : IEmailBusiness
{
    public IEmailData EmailData { get; }

    public EmailBusiness(IEmailData emailData)
    {
        EmailData = emailData;
    }


    public async Task<bool> EmailExists(string email)
    {
        email = email.Trim();

        if (email.Length < 1 || !email.Contains("@"))
        {
            return false;
        }
        return await EmailData.EmailExists(email);
    }
}
