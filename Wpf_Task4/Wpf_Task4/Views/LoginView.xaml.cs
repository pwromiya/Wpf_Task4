using System.Windows;
using Wpf_Task4.ViewModels;

namespace Wpf_Task4.Views;

// Login window code-behind
public partial class LoginView : Window
{
    public LoginView(LoginViewModel vm)
    {
        InitializeComponent();
        DataContext = vm; // Set ViewModel for data binding
    }

    // Handle login button click
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as LoginViewModel;
        if (vm != null)
        {
            // Pass password to ViewModel for authentication
            vm.LoginExecute(PasswordBox.Password);
            PasswordBox.Clear(); // Clear password field after login attempt
        }
    }
}