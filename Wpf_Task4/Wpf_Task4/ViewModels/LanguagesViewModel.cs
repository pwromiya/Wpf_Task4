using System.Windows.Input;
using Wpf_Task4.Commands;

namespace Wpf_Task4.ViewModels;

// ViewModel for language switching functionality
public class LanguageViewModel
{
    private readonly ILanguageService _languageService;

    // Commands for each language option
    public ICommand SetLangRuCommand { get; }
    public ICommand SetLangEnCommand { get; }
    public ICommand SetLangBeCommand { get; }

    public LanguageViewModel(ILanguageService languageService)
    {
        _languageService = languageService;

        // Initialize commands with corresponding language changes
        SetLangRuCommand = new RelayCommand(_ => _languageService.ChangeLanguage("ru"));
        SetLangEnCommand = new RelayCommand(_ => _languageService.ChangeLanguage("en"));
        SetLangBeCommand = new RelayCommand(_ => _languageService.ChangeLanguage("be"));
    }
}