namespace Wpf_Task4.Domain.Models;

// User Domain model
public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}