using System.Windows;
using System.Windows.Controls;

namespace Wpf_Task4.Views;

// Language switcher UserControl
public partial class LanguageSwitcher : UserControl
{
    public LanguageSwitcher()
    {
        InitializeComponent();

        // Set DataContext when control loads (LanguageViewModel from app resources)
        this.Loaded += (s, e) =>
        {
            if (Application.Current.Resources.Contains("LangVM"))
            {
                this.DataContext = Application.Current.Resources["LangVM"];
            }
        };
    }
}