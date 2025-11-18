using System.ComponentModel.DataAnnotations;

namespace Models;

public class User
{
    public int id { get; set; }
    public string name { get; set; }
    [EmailAddress]
    public string email { get; set; }
    public string password { get; set; }
    public string? imagePath { get; set; }
}
