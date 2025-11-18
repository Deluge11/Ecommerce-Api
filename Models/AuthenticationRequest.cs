using System.ComponentModel.DataAnnotations;

namespace Models;

public class AuthenticationRequest
{
    [EmailAddress]
    public string email { get; set; }
    public string password { get; set; }
}
