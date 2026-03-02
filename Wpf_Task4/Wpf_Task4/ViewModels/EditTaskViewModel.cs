using System.ComponentModel;
using System.Windows.Input;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Commands;
using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.UI.ViewModels;

// ViewModel for create/update task (EditTaskControl)
public class EditTaskViewModel : INotifyPropertyChanged
{
    private readonly ITaskService _taskService;  // The main logic service of this model
    private readonly IErrorHandler _errorHandler;   // Handles application errors
    private readonly int _taskId;
    private readonly int _projectId;
    private readonly bool _isEditMode;  // True if editing existing task

    public Array Statuses => Enum.GetValues(typeof(Domain.Models.TaskStatus));
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public Domain.Models.TaskStatus Status { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action? Saved; // Raised after successful save
    public event Action? Canceled; // Raised on cancel

    public EditTaskViewModel(
        ITaskService taskService,
        int projectId,
        ProjectTask? task = null,
        IErrorHandler errorHandler = null)
    {
        _taskService = taskService;
        _projectId = projectId;

        if (task != null)
        {
            _isEditMode = true;
            _taskId = task.Id;
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
        }

        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        CancelCommand = new RelayCommand(_ => Canceled?.Invoke());
        _errorHandler = errorHandler;
    }

    // Saves task (create or update)
    private async Task SaveAsync()
    {
        if (_isEditMode)
        {
            await _taskService.UpdateTaskAsync(
                _taskId,
                Title,
                Description,
                Status);
        }
        else
        {
            try
            {
                await _taskService.CreateTaskAsync(
                Title,
                Description,
                _projectId);
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
            }

        }

        Saved?.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}