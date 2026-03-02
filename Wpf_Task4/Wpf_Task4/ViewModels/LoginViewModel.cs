using System.ComponentModel;
using System.Windows.Input;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Commands;
using Wpf_Task4.UI.Services;

namespace Wpf_Task4.UI.ViewModels;

// ViewModel for user login functionality (LoginView)
public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUserService _userService; // The main logic service of this model
    private readonly IWindowService _windowService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;
    private readonly ICurrentUserService _currentUserService;   // Current user info

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

    public ICommand LoginCommand { get; }

    public LoginViewModel(
        IUserService userService,
        ICurrentUserService currentUserService,
        IWindowService windowService,
        IMessageService messageService,
        ILanguageService languageService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
        _windowService = windowService;
        _messageService = messageService;
        _languageService = languageService;

        LoginCommand = new RelayCommand(async param => await LoginAsync(param?.ToString()));
    }

    // Perform login
    public async Task LoginAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
        {
            _messageService.ShowWarning(_languageService.GetString("EnterCredentials"));
            return;
        }

        var user = await _userService.LoginAsync(Login, password);

        if (user != null)
        {
            _currentUserService.CurrentUser = user;
            _windowService.ShowMain();
            _windowService.ClosePrevious(); // Close login window
        }
        else
        {
            _messageService.ShowError(_languageService.GetString("InvalidLoginOrPassword"));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}