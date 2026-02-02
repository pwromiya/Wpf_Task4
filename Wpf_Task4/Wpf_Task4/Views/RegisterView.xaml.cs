using System.Windows;
using Wpf_Task4.ViewModels;

namespace Wpf_Task4.Views;

// Registration window code-behind
public partial class RegisterView : Window
{
    public RegisterView(RegisterViewModel vm)
    {
        InitializeComponent();
        DataContext = vm; // Set ViewModel for data binding
    }

    // Handle register button click (uses PasswordBox which can't be bound directly)
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as RegisterViewModel;
        if (vm != null)
        {
            vm.Register(PasswordBox.Password); // Pass password to ViewModel
            PasswordBox.Clear(); // Clear password field after submission
        }
    }
}