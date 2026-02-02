using Wpf_Task4.Models;

namespace Wpf_Task4.Services;

// Service interface for managing current user session
public interface ICurrentUserService
{
    User CurrentUser { get; set; } // Currently logged-in user
}