using Wpf_Task4.Application.Interfaces;
using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.Application.Services;

//Service for managing current user session
public class CurrentUserService : ICurrentUserService
{
    public User CurrentUser { get; set; }
}