using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wpf_Task4.Models;

// Task status enumeration
public enum TaskStatus
{
    Todo = 0,        // Queued for work
    InProgress = 1,  // Currently being worked on
    Review = 2,      // Under review/verification
    Blocked = 3,     // Blocked by dependencies/issues
    Done = 4,        // Completed successfully
    Cancelled = 5    // Cancelled/abandoned
}

// Project task model with INotifyPropertyChanged for UI binding
public class ProjectTask : INotifyPropertyChanged
{
    private TaskStatus _status;
    private string _title = "";
    private string? _description;

    public int Id { get; set; }

    [NotMapped] // Not stored in database
    public Action? StatusChangedCallback { get; set; } // Callback for status changes

    public TaskStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;

            _status = value;
            OnPropertyChanged(nameof(Status));

            // Trigger save to database immediately on status change
            StatusChangedCallback?.Invoke();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;

            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (_description == value) return;

            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public int ProjectId { get; set; }   // Foreign key to Project
    public DateTime CreatedAt { get; set; }

    public ProjectTask()
    {
        CreatedAt = DateTime.Now;

        // Initialize with Todo status (no callback trigger)
        _status = TaskStatus.Todo;
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}