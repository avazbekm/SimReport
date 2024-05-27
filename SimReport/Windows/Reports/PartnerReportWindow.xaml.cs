using System;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SimReport.Pages.Clients;

namespace SimReport.Windows.Reports;

/// <summary>
/// Interaction logic for PartnerReportWindow.xaml
/// </summary>
public partial class PartnerReportWindow : Window
{
    private readonly IServiceProvider services;
    public PartnerReportWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;

        List<Item> items = new List<Item>();
        var users = services.GetRequiredService<IUserService>().GetAllAsync().Result.Data;
        foreach (var user in users)
        {

            
            items.Add(new Item()
            {
                Id = user.Id,
                Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                Phone = user.Phone
            });
        }
        dataGrid.ItemsSource = items;
    }
}