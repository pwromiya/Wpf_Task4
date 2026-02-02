using Wpf_Task4.Models;

namespace Wpf_Task4.Services;

// Implementation of ICurrentUserService for managing current user session
public class CurrentUserService : ICurrentUserService
{
    public User CurrentUser { get; set; } // Stores the currently authenticated user
}