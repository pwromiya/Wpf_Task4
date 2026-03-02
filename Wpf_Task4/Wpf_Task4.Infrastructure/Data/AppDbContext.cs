using Microsoft.EntityFrameworkCore;
using Wpf_Task4.Domain.Models;

namespace Wpf_Task4.Infrastructure.Data;

// Main database context for the application
public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        // Create database if it doesn't exist
        Database.EnsureCreated();
    }
}