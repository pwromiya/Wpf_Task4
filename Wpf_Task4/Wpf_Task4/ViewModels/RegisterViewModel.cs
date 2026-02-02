using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Wpf_Task4.Commands;
using Wpf_Task4.Data;
using Wpf_Task4.Models;
using Wpf_Task4.Services;

namespace Wpf_Task4.ViewModels;

// ViewModel for user registration functionality
public class RegisterViewModel : INotifyPropertyChanged
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

    // Commands
    public ICommand RegisterCommand { get; }
    public ICommand OpenLoginCommand { get; }
    public ICommand SetLangRuCommand { get; }
    public ICommand SetLangEnCommand { get; }
    public ICommand SetLangBeCommand { get; }

    public RegisterViewModel(AppDbContext dbContext, IWindowService windowService,
                             ICurrentUserService currentUserService, ILanguageService lang)
    {
        // Language commands
        SetLangRuCommand = new RelayCommand(_ => lang.ChangeLanguage("ru"));
        SetLangEnCommand = new RelayCommand(_ => lang.ChangeLanguage("en"));
        SetLangBeCommand = new RelayCommand(_ => lang.ChangeLanguage("be"));

        _dbContext = dbContext;
        _windowService = windowService;
        _currentUserService = currentUserService;

        // Initialize commands with validation
        RegisterCommand = new RelayCommand(param => Register(param?.ToString()), CanRegister);
        OpenLoginCommand = new RelayCommand(obj => OpenLogin(obj));
    }

    // Register user with provided password (called from code-behind)
    public void Register(string password)
    {
        if (string.IsNullOrWhiteSpace(Login))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterLogin"));
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterPassword"));
            return;
        }

        // Check if login already exists
        if (_dbContext.Users.Any(u => u.Login == Login))
        {
            MessageBox.Show(LocalizationManager.GetString("UserAlreadyExists"));
            return;
        }

        // Generate salt and hash for password
        var salt = Guid.NewGuid().ToString();
        var user = new User
        {
            Login = Login,
            PasswordSalt = salt,
            PasswordHash = HashPassword(password, salt)
        };

        try
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            string successMessage = string.Format(LocalizationManager.GetString("RegistrationSuccess"), Login, user.Id);
            MessageBox.Show(successMessage);

            Login = string.Empty; // Clear login field
            OnPropertyChanged(nameof(Login));

            _currentUserService.CurrentUser = user; // Set current user session

            _windowService.CloseCurrent(); // Close registration window
            _windowService.ShowMain();     // Open main application window
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LocalizationManager.GetString("RegistrationError")}: {ex.Message}");
        }
    }

    // Validation for registration command
    private bool CanRegister(object parameter)
    {
        return !string.IsNullOrWhiteSpace(Login);
    }

    // Hash password with salt using SHA256
    private string HashPassword(string password, string salt)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToBase64String(bytes);
    }

    // Navigate to login window
    private void OpenLogin(object obj)
    {
        _windowService.CloseCurrent();  // Close registration window
        _windowService.ShowLogin();     // Open login window
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}