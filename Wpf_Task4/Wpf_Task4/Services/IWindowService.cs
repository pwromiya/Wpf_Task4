namespace Wpf_Task4.Services;

// Service interface for window management and navigation
public interface IWindowService
{
    void ShowRegister(); // Opens registration window
    void ShowLogin();    // Opens login window
    void CloseCurrent(); // Closes current window
    void ShowMain();     // Opens main application window
}