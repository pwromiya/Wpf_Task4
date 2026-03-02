using System.Windows;
using System.Windows.Controls;
using Wpf_Task4.UI.ViewModels;

namespace Wpf_Task4.UI.Views.UserControls;

public partial class EditProjectControl : UserControl
{
    public EditProjectControl()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.ProjectsVM.IsAddProjectPopupOpen = false;
    }
}
