using System.ComponentModel;
using System.Windows.Input;
using Wpf_Task4.Commands;

namespace Wpf_Task4.ViewModels;

// ViewModel for EditItemControl (generic editor for Project/Task)
public class EditItemViewModel : INotifyPropertyChanged
{
    private string _title = "";
    private string _description = "";

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(nameof(Title)); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(nameof(Description)); }
    }

    // Commands for save and cancel actions
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public EditItemViewModel(Action onSave, Action onCancel)
    {
        // Initialize commands with provided callbacks
        SaveCommand = new RelayCommand(_ => onSave());
        CancelCommand = new RelayCommand(_ => onCancel());
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}