using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimReport.Contants;
using SimReport.Entities.Users;
using SimReport.Interfaces;
using SimReport.Repositories;
using SimReport.Services;
using System.Windows;

namespace SimReport;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceCollection services = new ServiceCollection();

        // database 
        //services.AddDbContext<AppDbContext>(options =>
        //options.UseNpgsql("Host=localhost; Port=5432; User Id=postgres; Password=root; Database=SimReportDB;"));
        
        // Serivices
        services.AddScoped<IUserService, UserService>();

        // Repositories
        services.AddScoped<IRepository<User>, Repository<User>>();

        var serviceProvider = services.BuildServiceProvider();

        new MainWindow(serviceProvider).Show();
    }

}
