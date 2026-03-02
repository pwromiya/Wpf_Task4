using System.Windows;
using Wpf_Task4.UI.ViewModels;

namespace Wpf_Task4.UI.Views;

// Main application window
public partial class MainView : Window
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Share LanguageViewModel from application resources to this window
        if (System.Windows.Application.Current.Resources.Contains("LangVM"))
        {
            this.Resources["LangVM"] = System.Windows.Application.Current.Resources["LangVM"];
        }
    }
}