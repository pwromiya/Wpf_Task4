namespace Wpf_Task4.UI.Services;

// Service interface for window management and navigation
public interface IWindowService
{
    void ShowRegister();
    void ShowLogin();
    void CloseCurrent();
    void ClosePrevious();
    void ShowMain();
}