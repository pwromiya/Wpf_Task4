using Microsoft.EntityFrameworkCore;
using Wpf_Task4.Models;

namespace Wpf_Task4.Data;

// Main database context for the application
public class AppDbContext : DbContext
{
    // DbSet for User entities - represents Users table
    public DbSet<User> Users => Set<User>();

    // DbSet for Project entities - represents Projects table
    public DbSet<Project> Projects => Set<Project>();

    // DbSet for ProjectTask entities - represents ProjectTasks table
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

    // Constructor with dependency injection for DbContextOptions
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        // Create database on first run if it doesn't exist
        Database.EnsureCreated();
    }
}