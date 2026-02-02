using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Wpf_Task4.Commands;
using Wpf_Task4.Data;
using Wpf_Task4.Models;
using Wpf_Task4.Services;

namespace Wpf_Task4.ViewModels;

// Main ViewModel for the application's primary functionality
public class MainViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWindowService _windowService;

    // Current user information
    public User CurrentUser { get; }

    // Локализованная строка для текущего пользователя
    public string CurrentUserLogin => $"{LocalizationManager.GetString("CurUser")}: {CurrentUser.Login}";

    // ================= PROJECTS =================
    public ObservableCollection<Project> Projects { get; }

    private Project? _selectedProject;
    public Project? SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            OnPropertyChanged(nameof(SelectedProject));
            LoadTasks(); // Load tasks for the selected project
        }
    }

    // ================= TASKS =================
    public ObservableCollection<ProjectTask> ProjectTasks { get; } = new();
    public ObservableCollection<Models.TaskStatus> TaskStatuses { get; } =
        new(Enum.GetValues(typeof(Models.TaskStatus)).Cast<Models.TaskStatus>());

    // ================= NEW PROJECT =================
    private bool _isAddProjectPopupOpen;
    public bool IsAddProjectPopupOpen
    {
        get => _isAddProjectPopupOpen;
        set { _isAddProjectPopupOpen = value; OnPropertyChanged(nameof(IsAddProjectPopupOpen)); }
    }
    public string NewProjectName { get; set; } = "";
    public string NewProjectDescription { get; set; } = "";

    // ================= NEW TASK =================
    public string NewTaskTitle { get; set; } = "";
    public string NewTaskDescription { get; set; } = "";

    // ================= EDIT POPUP =================
    private bool _isEditItemPopupOpen;
    public bool IsEditItemPopupOpen
    {
        get => _isEditItemPopupOpen;
        set { _isEditItemPopupOpen = value; OnPropertyChanged(nameof(IsEditItemPopupOpen)); }
    }

    private EditItemViewModel? _editItemVM;
    public EditItemViewModel? EditItemVM
    {
        get => _editItemVM;
        set { _editItemVM = value; OnPropertyChanged(nameof(EditItemVM)); }
    }

    // ================= EDIT PROJECT =================
    private bool _isEditProjectPopupOpen;
    public bool IsEditProjectPopupOpen
    {
        get => _isEditProjectPopupOpen;
        set { _isEditProjectPopupOpen = value; OnPropertyChanged(nameof(IsEditProjectPopupOpen)); }
    }
    public Project? SelectedProjectForEdit { get; set; }

    private string _editProjectDescription = "";
    public string EditProjectDescription
    {
        get => _editProjectDescription;
        set { _editProjectDescription = value; OnPropertyChanged(nameof(EditProjectDescription)); }
    }

    // ================= EDIT TASK =================
    private bool _isEditProjectTaskPopupOpen;
    public bool IsEditProjectTaskPopupOpen
    {
        get => _isEditProjectTaskPopupOpen;
        set { _isEditProjectTaskPopupOpen = value; OnPropertyChanged(nameof(IsEditProjectTaskPopupOpen)); }
    }
    public ProjectTask? SelectedProjectTaskForEdit { get; set; }

    private string _editProjectTaskDescription = "";
    public string EditProjectTaskDescription
    {
        get => _editProjectTaskDescription;
        set { _editProjectTaskDescription = value; OnPropertyChanged(nameof(EditProjectTaskDescription)); }
    }

    // ================= COMMANDS =================
    public ICommand ShowAddProjectPopupCommand { get; }
    public ICommand AddProjectCommand { get; }

    public ICommand AddTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand EditTaskCommand { get; }

    public ICommand DeleteProjectCommand { get; }
    public ICommand EditProjectCommand { get; }
    public ICommand SaveProjectCommand { get; }
    public ICommand CancelEditProjectCommand { get; }

    public ICommand SaveProjectTaskCommand { get; }
    public ICommand CancelEditProjectTaskCommand { get; }

    public ICommand SaveUserProfileCommand { get; }
    public ICommand LogoutCommand { get; }

    // ================= CONSTRUCTOR =================
    public MainViewModel(AppDbContext db, ICurrentUserService currentUserService, IWindowService windowService)
    {
        _db = db;
        _currentUserService = currentUserService;
        _windowService = windowService;

        CurrentUser = _currentUserService.CurrentUser;

        // Load user's projects
        Projects = new ObservableCollection<Project>(
            _db.Projects.Where(p => p.UserId == CurrentUser.Id).ToList()
        );

        // Initialize commands
        ShowAddProjectPopupCommand = new RelayCommand(_ => IsAddProjectPopupOpen = true);
        AddProjectCommand = new RelayCommand(AddProject);

        AddTaskCommand = new RelayCommand(AddTask, _ => SelectedProject != null);
        DeleteTaskCommand = new RelayCommand(DeleteTask);
        EditTaskCommand = new RelayCommand(EditTask);

        DeleteProjectCommand = new RelayCommand(DeleteProject);
        EditProjectCommand = new RelayCommand(EditProject);
        SaveProjectCommand = new RelayCommand(SaveProject);
        CancelEditProjectCommand = new RelayCommand(_ => IsEditProjectPopupOpen = false);

        SaveProjectTaskCommand = new RelayCommand(SaveProjectTask);
        CancelEditProjectTaskCommand = new RelayCommand(_ => IsEditProjectTaskPopupOpen = false);

        SaveUserProfileCommand = new RelayCommand(SaveUserProfile);
        LogoutCommand = new RelayCommand(Logout);
    }

    // ================= LOGOUT =================
    private void Logout(object? obj)
    {
        _windowService.CloseCurrent(); // Close main window
        _windowService.ShowLogin();    // Show login window
    }

    // ================= SAVE PASSWORD =================
    private void SaveUserProfile(object? obj)
    {
        if (obj is not System.Windows.Controls.PasswordBox pb)
            return;

        string newPassword = pb.Password;
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterNewPassword"));
            return;
        }

        try
        {
            // Generate new salt and hash for password
            string newSalt = Guid.NewGuid().ToString();
            string newHash = HashPassword(newPassword, newSalt);

            CurrentUser.PasswordSalt = newSalt;
            CurrentUser.PasswordHash = newHash;

            _db.Users.Update(CurrentUser);
            _db.SaveChanges();

            MessageBox.Show(LocalizationManager.GetString("PasswordUpdated"));
            pb.Password = ""; // Clear password box
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{LocalizationManager.GetString("ErrorSavingPassword")}: {ex.Message}");
        }

        // Local helper function for password hashing
        static string HashPassword(string password, string salt)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
            return Convert.ToBase64String(bytes);
        }
    }

    // ================= PROJECT LOGIC =================
    private void DeleteProject(object? obj)
    {
        if (obj is not Project project) return;

        string message = string.Format(LocalizationManager.GetString("DeleteProjectConfirm"), project.Name);
        if (MessageBox.Show(message,
                           LocalizationManager.GetString("Confirm"),
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            return;

        // Remove all tasks for this project
        var tasksToRemove = _db.ProjectTasks.Where(t => t.ProjectId == project.Id).ToList();
        _db.ProjectTasks.RemoveRange(tasksToRemove);

        // Remove the project itself
        _db.Projects.Remove(project);
        _db.SaveChanges();

        foreach (var t in tasksToRemove)
            ProjectTasks.Remove(t);

        Projects.Remove(project);

        if (SelectedProject == project)
            SelectedProject = null;
    }

    private void AddProject(object? _)
    {
        if (string.IsNullOrWhiteSpace(NewProjectName))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterProjectName"));
            return;
        }

        var project = new Project
        {
            Name = NewProjectName,
            Description = NewProjectDescription,
            UserId = CurrentUser.Id,
            CreatedAt = DateTime.Now
        };

        _db.Projects.Add(project);
        _db.SaveChanges();
        Projects.Add(project);

        // Clear form fields and close popup
        NewProjectName = "";
        NewProjectDescription = "";
        IsAddProjectPopupOpen = false;

        // Уведомляем об изменении свойств
        OnPropertyChanged(nameof(NewProjectName));
        OnPropertyChanged(nameof(NewProjectDescription));
    }

    private void EditProject(object? obj)
    {
        if (obj is not Project project) return;

        // Initialize edit dialog ViewModel
        EditItemVM = new EditItemViewModel(
            onSave: () =>
            {
                project.Description = EditItemVM!.Description;
                _db.SaveChanges();
                IsEditItemPopupOpen = false;
            },
            onCancel: () => IsEditItemPopupOpen = false
        );

        EditItemVM.Title = LocalizationManager.GetString("EditProject");
        EditItemVM.Description = project.Description ?? "";
        IsEditItemPopupOpen = true;
    }

    private void SaveProject(object? _)
    {
        if (SelectedProjectForEdit == null) return;

        SelectedProjectForEdit.Description =
            string.IsNullOrWhiteSpace(EditProjectDescription) ? null : EditProjectDescription;

        _db.Projects.Update(SelectedProjectForEdit);
        _db.SaveChanges();
        IsEditProjectPopupOpen = false;
    }

    // ================= TASK LOGIC =================
    private void LoadTasks()
    {
        ProjectTasks.Clear();
        if (SelectedProject == null) return;

        // Load tasks for selected project with status change callback
        foreach (var task in _db.ProjectTasks.Where(t => t.ProjectId == SelectedProject.Id))
        {
            task.StatusChangedCallback = () => UpdateTaskStatus(task);
            ProjectTasks.Add(task);
        }
    }

    private void AddTask(object? _)
    {
        if (SelectedProject == null) return;

        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            MessageBox.Show(LocalizationManager.GetString("EnterTaskTitle"));
            return;
        }

        var task = new ProjectTask
        {
            Title = NewTaskTitle,
            Description = string.IsNullOrWhiteSpace(NewTaskDescription) ? null : NewTaskDescription,
            Status = Models.TaskStatus.Todo,
            ProjectId = SelectedProject.Id,
            CreatedAt = DateTime.Now
        };

        task.StatusChangedCallback = () => UpdateTaskStatus(task);

        _db.ProjectTasks.Add(task);
        _db.SaveChanges();
        ProjectTasks.Add(task);

        // Clear form fields
        NewTaskTitle = "";
        NewTaskDescription = "";
        OnPropertyChanged(nameof(NewTaskTitle));
        OnPropertyChanged(nameof(NewTaskDescription));
    }

    private void DeleteTask(object? obj)
    {
        if (obj is not ProjectTask task) return;

        string message = string.Format(LocalizationManager.GetString("DeleteTaskConfirm"), task.Title);
        if (MessageBox.Show(message,
                           LocalizationManager.GetString("Confirm"),
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            return;

        _db.ProjectTasks.Remove(task);
        _db.SaveChanges();
        ProjectTasks.Remove(task);
    }

    private void EditTask(object? obj)
    {
        if (obj is not ProjectTask task) return;

        // Initialize edit dialog ViewModel
        EditItemVM = new EditItemViewModel(
            onSave: () =>
            {
                task.Description = EditItemVM!.Description;
                _db.SaveChanges();
                IsEditItemPopupOpen = false;
            },
            onCancel: () => IsEditItemPopupOpen = false
        );

        EditItemVM.Title = LocalizationManager.GetString("EditTask");
        EditItemVM.Description = task.Description ?? "";
        IsEditItemPopupOpen = true;
    }

    private void SaveProjectTask(object? _)
    {
        if (SelectedProjectTaskForEdit == null) return;

        SelectedProjectTaskForEdit.Description =
            string.IsNullOrWhiteSpace(EditProjectTaskDescription) ? null : EditProjectTaskDescription;

        _db.ProjectTasks.Update(SelectedProjectTaskForEdit);
        _db.SaveChanges();
        IsEditProjectTaskPopupOpen = false;
    }

    // Update task status in database when changed via UI
    private void UpdateTaskStatus(ProjectTask task)
    {
        _db.ProjectTasks.Update(task);
        _db.SaveChanges();
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}