using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.Application.Interfaces;

// Service interface for managing current user session
public interface ICurrentUserService
{
    User CurrentUser { get; set; }
}