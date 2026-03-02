namespace Wpf_Task4.Application.Interfaces;

// Service interface for managing application language
public interface ILanguageService
{
    string GetString(string key);
    void ChangeLanguage(string culture);
}