using System.ComponentModel;
using System.Windows.Input;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Commands;
using Wpf_Task4.Domain.Common;
using Wpf_Task4.UI.Services;

namespace Wpf_Task4.UI.ViewModels;

// ViewModel for user registration functionality (RegisterView)

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly IUserService _userService; // The main logic service of this model
    private readonly IWindowService _windowService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService; // Current user info

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

    public ICommand RegisterCommand { get; }
    public ICommand OpenLoginCommand { get; }
    public ICommand SetLangRuCommand { get; }
    public ICommand SetLangEnCommand { get; }
    public ICommand SetLangBeCommand { get; }

    public RegisterViewModel(
        IUserService userService,
        IWindowService windowService,
        IMessageService messageService,
        ILanguageService languageService)
    {
        _userService = userService;
        _windowService = windowService;
        _messageService = messageService;
        _languageService = languageService;

        RegisterCommand = new RelayCommand(async param => await RegisterAsync(param?.ToString()));
        OpenLoginCommand = new RelayCommand(_ =>
        {
            _windowService.ShowLogin();
            _windowService.ClosePrevious(); // Close register window
        });

        // Language commands
        SetLangRuCommand = new RelayCommand(_ => _languageService.ChangeLanguage("ru"));
        SetLangEnCommand = new RelayCommand(_ => _languageService.ChangeLanguage("en"));
        SetLangBeCommand = new RelayCommand(_ => _languageService.ChangeLanguage("be"));
    }

    // Perform registration
    public async Task RegisterAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(Login))
        {
            _messageService.ShowWarning(_languageService.GetString("EnterLogin"));
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _messageService.ShowWarning(_languageService.GetString("EnterPassword"));
            return;
        }

        try
        {
            // Informing the user about registration by message service
            var user = await _userService.RegisterAsync(Login, password);
            _messageService.ShowInformation(
                string.Format(_languageService.GetString("RegistrationSuccess"), user.Login, user.Id));

            _windowService.ShowMain();
            _windowService.ClosePrevious();  // Close register window
        }
        catch (AppException ex)
        {
            _messageService.ShowWarning(_languageService.GetString(ex.UserMessage));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}