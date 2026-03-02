using System.Windows;
using Wpf_Task4.UI.ViewModels;

namespace Wpf_Task4.UI.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel vm)
        {
            InitializeComponent();
            DataContext = vm; // Устанавливаем VM для биндинга
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                // Передаём пароль из PasswordBox в метод VM
                vm.RegisterAsync(PasswordBox.Password);

                // Очищаем поле пароля
                PasswordBox.Clear();
            }
        }

    }
}