using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Wpf_Task4.Data;
using Wpf_Task4.Services;
using Wpf_Task4.ViewModels;
using Wpf_Task4.Views;

namespace Wpf_Task4;

// Main application class with DI configuration
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configure Dependency Injection
        var services = new ServiceCollection();

        // DbContext with SQL Server connection
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<MainViewModel>();

        // Views
        services.AddTransient<RegisterView>();
        services.AddTransient<MainView>();

        // Services
        services.AddTransient<LoginView>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        // Language services
        services.AddSingleton<LanguageViewModel>();
        services.AddSingleton<ILanguageService, LanguageService>();

        // Build service provider
        ServiceProvider = services.BuildServiceProvider();

        // Add LanguageViewModel to application resources for global access
        Current.Resources.Add("LangVM", ServiceProvider.GetRequiredService<LanguageViewModel>());

        // Launch startup window (Registration)
        var window = ServiceProvider.GetRequiredService<RegisterView>();
        window.Show();
    }
}