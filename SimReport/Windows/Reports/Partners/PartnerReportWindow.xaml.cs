using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Windows.Reports.Partners;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports;

/// <summary>
/// Interaction logic for PartnerReportWindow.xaml
/// </summary>
public partial class PartnerReportWindow : Window
{
    private readonly IUserService userService;
    private readonly ICardService cardService;
    private readonly ICompanyService companyService;
    public PartnerReportWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.userService = services.GetRequiredService<IUserService>();
        this.cardService = services.GetRequiredService<ICardService>();
        this.companyService = services.GetRequiredService<ICompanyService>();

        dataGrid.ItemsSource = GetAllUser();
    }

    private List<ItemReport> GetAllUser()
    {
        // sim kartalar hammasini olib kelyapmiz
        
        List<ItemReport> items = new List<ItemReport>();
        // hamkorlarni hammasi olib kelyapmiz
        var users = userService.GetAllAsync().Result.Data.ToList();
        
            foreach (var user in users)
            {
                int mobiuz = 0;
                int beeline = 0;
                int ucell = 0;
                int uzmobayl = 0;
                int humans = 0;
                int quantitiy = 0;
                
                var cards = cardService.GetAllAsync(user.Phone).Result.Data;

                Dictionary<int, int> companyQuantity = new Dictionary<int, int>();

                if (cards is not null)
                {
                    quantitiy = cards.Count();

                    foreach (var card in cards)
                        {
                            if (companyQuantity.Keys.Contains(card.CompanyId))
                                companyQuantity[card.CompanyId]++;
                            else
                                companyQuantity.Add(card.CompanyId, 1);
                        }
                }

                foreach (var item in companyQuantity.Keys)
                {
                    var companyName = companyService.GetAsync(item).Result.Data;

                    if (companyName.Name.Equals("mobiuz"))
                        mobiuz = companyQuantity[item];
                    else if (companyName.Name.Equals("beeline"))
                        beeline = companyQuantity[item];
                    else if (companyName.Name.Equals("ucell"))
                        ucell = companyQuantity[item];
                    else if (companyName.Name.Equals("uzmobayl"))
                        uzmobayl = companyQuantity[item];
                    else if (companyName.Name.Equals("humans"))
                        humans = companyQuantity[item];
                }

                items.Add(new ItemReport()
                {
                    Id = user.Id,
                    Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                    Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                    Phone = user.Phone,
                    Mobiuz = mobiuz,
                    Beeline = beeline,
                    Ucell = ucell,
                    Uzmobayl = uzmobayl,
                    Humans = humans,
                    Quantity = quantitiy
                });
            }
            return items;
    }
}