using System.ComponentModel;
using System.Windows.Input;
using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Commands;
using Wpf_Task4.Domain.Common;
using Wpf_Task4.Domain.Models;
using Wpf_Task4.UI.Services;

namespace Wpf_Task4.UI.ViewModels;

//MainViewModel orchestrates projects (ProjectsViewModel), tasks (TasksViewModel), and user interactions in the main window (MainView)

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IWindowService _windowService;
    private readonly ICurrentUserService _currentUserService;   
    private readonly IErrorHandler _errorHandler;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;
    private object? _currentDialog;


    public User CurrentUser { get; }  // Current logged-in user
    public ProjectsViewModel ProjectsVM { get; }  // Sub-VM managing projects
    public TasksViewModel TasksVM { get; }  // Sub-VM managing projects

    public ICommand LogoutCommand { get; }
    public ICommand SaveUserProfileCommand { get; } // Change password command
    
    // For opening EditTaskControl/EditProjectControl
    public object? CurrentDialog
    {
        get => _currentDialog;
        set { _currentDialog = value; OnPropertyChanged(nameof(CurrentDialog)); }
    }
    public ICommand ShowAddProjectCommand { get; }
    public ICommand ShowEditProjectCommand { get; }

    public ICommand ShowAddTaskCommand { get; }
    public ICommand ShowEditTaskCommand { get; }
    public MainViewModel(
        ProjectsViewModel projectsVM,
        TasksViewModel tasksVM,
        IWindowService windowService,
        ICurrentUserService currentUserService,
        IErrorHandler errorHandler,
        IUserService userService,
        IMessageService messageService)
    {
        ProjectsVM = projectsVM;
        TasksVM = tasksVM;
        _windowService = windowService;
        _currentUserService = currentUserService;
        _errorHandler = errorHandler;
        CurrentUser = _currentUserService.CurrentUser;
        _userService = userService;
        _messageService = messageService;

        // Sync tasks with selected project
        ProjectsVM.PropertyChanged += OnProjectsPropertyChanged;

        LogoutCommand = new RelayCommand(_ => Logout());

        // Change password command
        SaveUserProfileCommand = new RelayCommand(async param =>
        {
            if (param is System.Windows.Controls.PasswordBox pb)
            {
                string newPassword = pb.Password;
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    _messageService.ShowWarning(LocalizationManager.GetString("EnterPassword"));
                    return;
                }
                try
                {
                    await _userService.ChangePasswordAsync(CurrentUser.Id, newPassword);
                    _messageService.ShowInformation(LocalizationManager.GetString("PasswordUpdated"));
                    pb.Clear();
                }
                catch (AppException ex)
                {
                    _messageService.ShowWarning(ex.UserMessage);
                }
                catch (Exception ex)
                {
                    _errorHandler.Handle(ex);
                }
            }
        });

        // Commands to open project/task editor popups
        ShowAddProjectCommand = new RelayCommand(_ => ShowEditProject(null));
        ShowEditProjectCommand = new RelayCommand(p => ShowEditProject(p as Project));

        ShowAddTaskCommand = new RelayCommand(_ => ShowEditTask(null));
        ShowEditTaskCommand = new RelayCommand(t => ShowEditTask(t as ProjectTask));

    }

    private void OnProjectsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProjectsViewModel.SelectedProject))
        {
            TasksVM.SelectedProject = ProjectsVM.SelectedProject; // Propagate selection change
        }
    }

    private void Logout()
    {
        _windowService.ShowRegister();
        _windowService.ClosePrevious(); // Close main window
    }

    // Show project editor Usercontrol
    public void ShowEditProject(Project? project)
    {
        var vm = new EditProjectViewModel(ProjectsVM.ProjectService, project, _errorHandler);
        vm.Saved += async () =>
        {
            CurrentDialog = null;
            await ProjectsVM.LoadProjectsAsync();
        };
        vm.Canceled += () => CurrentDialog = null;

        CurrentDialog = vm;
    }

    // Show task editor popup
    public void ShowEditTask(ProjectTask? task)
    {
        if (TasksVM.SelectedProject == null) return;

        var vm = new EditTaskViewModel(TasksVM.TaskService, TasksVM.SelectedProject.Id, task, _errorHandler);
        vm.Saved += async () =>
        {
            CurrentDialog = null;
            await TasksVM.LoadTasksAsync(TasksVM.SelectedProject);
        };
        vm.Canceled += () => CurrentDialog = null;

        CurrentDialog = vm;
    }

    // Event for INotifyPropertyChanged; notifies UI when a property changes
    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to raise PropertyChanged event
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}