using System.Windows;

// Implementation for changing application language at runtime
public class LanguageService : ILanguageService
{
    public void ChangeLanguage(string culture)
    {
        // Create ResourceDictionary for the specified culture/language
        var dict = new ResourceDictionary
        {
            Source = new Uri(
                $"/Resources/Languages/Strings.{culture}.xaml", // Path to language resource file
                UriKind.Relative)
        };

        // Replace current language resources with new ones
        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
}