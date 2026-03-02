using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.Application.Interfaces;

// Interface for CRUD with Task
public interface ITaskService
{
    Task<ProjectTask> CreateTaskAsync(string title, string? description, int projectId);
    Task<List<ProjectTask>> GetByProjectIdAsync(int projectId);
    Task<ProjectTask?> GetByIdAsync(int taskId);
    Task UpdateTaskAsync(int taskId, string? title, string? description, Domain.Models.TaskStatus? status);
    Task DeleteTaskAsync(int taskId);

    // Business logic
    Task<bool> CanUserModifyTaskAsync(int taskId, int userId);

}