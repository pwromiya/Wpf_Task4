using System.ComponentModel;
using System.Text;
using System.Windows;
using Wpf_Task4.Data;
using Wpf_Task4.Services;
using System.Security.Cryptography;

namespace Wpf_Task4.ViewModels;

// ViewModel for login functionality
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _dbContext;
    private readonly IWindowService _windowService;
    private readonly ICurrentUserService _currentUserService;

    private string _login;
    public string Login
    {
        get => _login;
        set
        {
            _login = value;
            OnPropertyChanged(nameof(Login));
        }
    }

    public LoginViewModel(AppDbContext db, IWindowService ws, ICurrentUserService cu)
    {
        _dbContext = db;
        _windowService = ws;
        _currentUserService = cu;
    }

    // Execute login with provided password (called from code-behind)
    public void LoginExecute(string password)
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterCredentials"));
            return;
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Login == Login);
        if (user != null && HashPassword(password, user.PasswordSalt) == user.PasswordHash)
        {
            _currentUserService.CurrentUser = user; // Set current user session
            _windowService.CloseCurrent();          // Close login window
            _windowService.ShowMain();              // Open main application window
        }
        else
        {
            MessageBox.Show(LocalizationManager.GetString("InvalidLoginOrPassword"));
        }
    }

    // Hash password with salt using SHA256
    private string HashPassword(string password, string salt)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToBase64String(bytes);
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}