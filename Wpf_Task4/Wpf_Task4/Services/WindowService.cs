using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Wpf_Task4.Views;

namespace Wpf_Task4.Services;

// Window navigation service implementation
public class WindowService : IWindowService
{
    private Window _currentWindow; // Track currently opened window

    public void ShowMain()
    {
        var window = App.ServiceProvider.GetRequiredService<MainView>();
        _currentWindow = window;
        window.Show();
    }

    public void ShowRegister()
    {
        var window = App.ServiceProvider.GetRequiredService<RegisterView>();
        _currentWindow = window;
        window.Show();
    }

    public void ShowLogin()
    {
        var window = App.ServiceProvider.GetRequiredService<LoginView>();
        _currentWindow = window;
        window.Show();
    }

    public void CloseCurrent()
    {
        _currentWindow?.Close(); // Close current window if exists
    }
}