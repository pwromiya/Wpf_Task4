using System.ComponentModel;

namespace Wpf_Task4.Models;

// Project model with INotifyPropertyChanged for UI binding
public class Project : INotifyPropertyChanged
{
    private string? _description; // Backing field for Description

    public int Id { get; set; }                    // Primary key
    public string Name { get; set; } = "";         // Project name
    public int UserId { get; set; }                // Foreign key to User
    public DateTime CreatedAt { get; set; }        // Creation timestamp

    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description)); // Notify UI of change
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to raise PropertyChanged event
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}