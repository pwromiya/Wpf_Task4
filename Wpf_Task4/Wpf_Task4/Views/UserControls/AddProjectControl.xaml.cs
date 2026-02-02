using System.Windows;
using System.Windows.Controls;

namespace Wpf_Task4.Views.UserControls;

public partial class AddProjectControl : UserControl
{
    public AddProjectControl()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
            vm.IsAddProjectPopupOpen = false;
    }
}
