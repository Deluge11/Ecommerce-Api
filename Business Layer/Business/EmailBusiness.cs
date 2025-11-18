
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


    public Task<bool> EmailExists(string email)
    {
        return EmailData.EmailExists(email);
    }
}
