using System.Windows.Input;

namespace Wpf_Task4.Commands;

// RelayCommand for WPF MVVM - connects UI controls to ViewModel methods
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;        // Main execution logic
    private readonly Func<object, bool> _canExecute; // Optional condition check

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        // Command is executable if no condition or condition returns true
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter); // Run the command action
    }

    public event EventHandler CanExecuteChanged
    {
        // Use WPF's CommandManager for automatic UI updates
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}