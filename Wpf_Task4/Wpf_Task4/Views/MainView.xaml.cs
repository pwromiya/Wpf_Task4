using System.Windows;
using Wpf_Task4.ViewModels;

namespace Wpf_Task4.Views;

// Main application window
public partial class MainView : Window
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Share LanguageViewModel from application resources to this window
        if (Application.Current.Resources.Contains("LangVM"))
        {
            this.Resources["LangVM"] = Application.Current.Resources["LangVM"];
        }
    }
}