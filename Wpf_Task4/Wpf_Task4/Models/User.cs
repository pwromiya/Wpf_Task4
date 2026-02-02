namespace Wpf_Task4.Models;

// User model for authentication and user data
public class User
{
    public int Id { get; set; }            // Primary key
    public string Login { get; set; }      // Username for login
    public string PasswordHash { get; set; } // Hashed password
    public string PasswordSalt { get; set; } // Salt for password hashing
}