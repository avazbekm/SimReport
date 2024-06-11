using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports;

/// <summary>
/// Interaction logic for PartnerReportWindow.xaml
/// </summary>
public partial class PartnerReportWindow : Window
{
    private readonly IUserService userService;
    public PartnerReportWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.userService = services.GetRequiredService<IUserService>();

        dataGrid.ItemsSource = GetAllUser();

    }

    private List<ItemReport> GetAllUser()
    {
        List<ItemReport> items = new List<ItemReport>();
        var users = userService.GetAllAsync().Result.Data.ToList();

        foreach (var user in users)
        {
            items.Add(new ItemReport()
            {
                Id = user.Id,
                Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                Phone = user.Phone
            });
        }
        return items;
    }
}