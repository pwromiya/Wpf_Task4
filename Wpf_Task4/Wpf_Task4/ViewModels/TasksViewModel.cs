using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Commands;
using Wpf_Task4.Domain.Models;
using Wpf_Task4.UI.Services;

namespace Wpf_Task4.UI.ViewModels;

// Manages tasks list for selected project (WorkSpaceView)
public class TasksViewModel : BaseViewModel
{
    public ITaskService TaskService { get; }  // Tasks main logic
    private readonly IMessageService _messageService; // User notifications

    public ObservableCollection<ProjectTask> ProjectTasks { get; } = new();

    private ProjectTask? _selectedTask;
    public ProjectTask? SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();  // Notify UI of selection change
        }
    }

    private Project? _selectedProject;
    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged();
            _ = LoadTasksAsync(); // Auto-load tasks on project change
        }
    }
    public ICommand DeleteTaskCommand { get; }

    public TasksViewModel(
        ITaskService taskService,
        IMessageService messageService)
    {
        TaskService = taskService;
        _messageService = messageService;

        // Bind delete command with task parameter
        DeleteTaskCommand = new RelayCommand(async parameter =>
        {
            if (parameter is ProjectTask task)
                await DeleteTaskAsync(task);
        });
    }

    // Load tasks for project
    public async Task LoadTasksAsync(Project? project = null)
    {
        ProjectTasks.Clear(); // Clear current list

        var targetProject = project ?? SelectedProject;
        if (targetProject == null)
            return;

        var tasks = await TaskService.GetByProjectIdAsync(targetProject.Id);
        foreach (var t in tasks)
            ProjectTasks.Add(t); // Populate observable collection
    }

    private async Task DeleteTaskAsync(ProjectTask task)
    {
        if (task == null)
            return;

        // Delete confirmation
        if (!_messageService.Confirm(
        string.Format(LocalizationManager.GetString("DeleteTaskConfirm"),task.Title),
        LocalizationManager.GetString("Del")))
            return;

        await TaskService.DeleteTaskAsync(task.Id);

        ProjectTasks.Remove(task); // Remove task locally without full reload
    }
}