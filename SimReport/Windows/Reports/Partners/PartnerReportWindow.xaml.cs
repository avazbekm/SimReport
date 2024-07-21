using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Reports.Partners;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports;

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

        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            dataGrid.ItemsSource = await GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<ItemReport>> GetAllUsersAsync()
    {
        List<ItemReport> items = new List<ItemReport>();

        // Retrieve all users
        var users = await userService.GetAllAsync();
        var userList = users.Data.ToList();

        foreach (var user in userList)
        {
            int mobiuz = 0;
            int beeline = 0;
            int ucell = 0;
            int uzmobile = 0;
            int humans = 0;
            int quantity = 0;

            var cards = await cardService.GetAllAsync(user.Phone);
            var cardList = cards.Data;

            Dictionary<int, int> companyQuantity = new Dictionary<int, int>();

            if (cardList != null)
            {
                quantity = cardList.Count();

                foreach (var card in cardList)
                {
                    if (companyQuantity.ContainsKey(card.CompanyId))
                        companyQuantity[card.CompanyId]++;
                    else
                        companyQuantity.Add(card.CompanyId, 1);
                }
            }

            foreach (var item in companyQuantity.Keys)
            {
                var company = await companyService.GetAsync(item);
                var companyName = company.Data.Name;

                switch (companyName.ToLower())
                {
                    case "mobiuz":
                        mobiuz = companyQuantity[item];
                        break;
                    case "beeline":
                        beeline = companyQuantity[item];
                        break;
                    case "ucell":
                        ucell = companyQuantity[item];
                        break;
                    case "uzmobayl":
                        uzmobile = companyQuantity[item];
                        break;
                    case "humans":
                        humans = companyQuantity[item];
                        break;
                }
            }

            items.Add(new ItemReport
            {
                Id = user.Id,
                Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                Phone = user.Phone,
                Mobiuz = mobiuz,
                Beeline = beeline,
                Ucell = ucell,
                Uzmobile = uzmobile,
                Humans = humans,
                Quantity = quantity
            });
        }

        return items;
    }
}
