using System.Windows;
using Wpf_Task4.UI.ViewModels;

namespace Wpf_Task4.UI.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm; // Устанавливаем VM для биндинга
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                // Передаём пароль из PasswordBox в метод VM
                vm.LoginAsync(PasswordBox.Password);

                // Очищаем поле пароля
                PasswordBox.Clear();
            }
        }
    }
}