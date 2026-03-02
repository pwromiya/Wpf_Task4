using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.Application.Interfaces;

// Interface for CRUD with Project
public interface IProjectService
{
    Task<Project> CreateProjectAsync(string name, string? description);
    Task<List<Project>> GetUserProjectsAsync(int userId);
    Task<Project?> GetByIdAsync(int projectId);
    Task UpdateProjectAsync(int projectId,string Name,string Description);
    Task DeleteProjectAsync(int projectId);

}