﻿using System.Windows;
using SimReport.Services;
using SimReport.Interfaces;
using SimReport.Repositories;
using SimReport.Entities.Users;
using SimReport.Entities.Cards;
using SimReport.Services.Helpers;
using SimReport.Entities.Companies;
using Microsoft.Extensions.DependencyInjection;

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

        // Serivices
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICardService, CardService>();
        //services.AddScoped<IAssetService, AssetService>();



        // Repositories
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Company>, Repository<Company>>();
        services.AddScoped<IRepository<Card>, Repository<Card>>();
        //services.AddScoped<IRepository<Asset>, Repository<Asset>>();

        PathHelper.ImagePath = System.IO.Path.GetFullPath("Assets");

        var serviceProvider = services.BuildServiceProvider();

        new MainWindow(serviceProvider).Show();
    }

}
