﻿using System;
using System.Linq;
using System.Windows;
using SimReport.Services;
using SimReport.Contants;
using SimReport.Interfaces;
using SimReport.Repositories;
using SimReport.Entities.Block;
using SimReport.Entities.Users;
using SimReport.Entities.Cards;
using SimReport.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IServiceCollection services = new ServiceCollection();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IBlockService, BlockService>();
        //services.AddScoped<IAssetService, AssetService>();



        // Repositories
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<Company>, Repository<Company>>();
        services.AddScoped<IRepository<Card>, Repository<Card>>();
        services.AddScoped<IRepository<BlockDate>, Repository<BlockDate>>();
        //services.AddScoped<IRepository<Asset>, Repository<Asset>>();


        // Add AppDbContext to the service collection
        services.AddDbContext<AppDbContext>();

        var serviceProvider = services.BuildServiceProvider();

        // Apply migrations on startup
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();  // This will automatically apply migrations on startup
        }

        new MainWindow(serviceProvider).Show();

        IBlockService blockService = serviceProvider.GetService<IBlockService>();
        var result = blockService.GetAllAsync().Result.Data.ToList();
        if (result.Count().Equals(0))
        {
            BlockDate blockDate = new BlockDate();
            blockDate.EndDate = DateTime.UtcNow.AddMonths(2);
            await blockService.AddAsync(blockDate);
        }
    }
}
