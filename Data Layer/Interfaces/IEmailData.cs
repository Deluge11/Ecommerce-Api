using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace Data_Layer.Interfaces;

public interface IEmailData
{
    Task<bool> EmailExists(string email);
}
