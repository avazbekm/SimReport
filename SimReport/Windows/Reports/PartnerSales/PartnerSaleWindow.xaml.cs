using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports.PartnerSales;

/// <summary>
/// Interaction logic for PartnerSaleWindow.xaml
/// </summary>
public partial class PartnerSaleWindow : Window
{
    private readonly IUserService userService;
    private readonly ICardService cardService;
    private readonly ICompanyService companyService;

    private int CompanyId;
    public PartnerSaleWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.userService = services.GetRequiredService<IUserService>();
        this.cardService = services.GetRequiredService<ICardService>();
        this.companyService = services.GetRequiredService<ICompanyService>();

        // Databazadan malumotlarni olish
        List<ItemComboBox> items = GetItemsFromDatabase();

        // Comboboxga malumotlarni yuklash
        cbCompany.ItemsSource = items;
    }

    private List<ItemComboBox> GetItemsFromDatabase()
    {
        List<ItemComboBox> items = new List<ItemComboBox>();
        items.Add(new ItemComboBox { Name = "Kompaniya tanlash" });
        var companies = companyService.GetAllAsync().Result.Data.ToList();
        if (companies is not null)
        {
            foreach (var company in companies)
            {
                items.Add(new ItemComboBox()
                {
                    Id = company.Id,
                    Name = ConvertToStandart.ConvertFirstToUpper(company.Name),
                });
            };
        }
        return items;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            dataGrid.ItemsSource = await GetAllCardsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<ItemPartnerSale>> GetAllCardsAsync()
    {
        Dictionary<int, string> companiesList = new Dictionary<int, string>();
                var companies = companyService.GetAllAsync().Result.Data.ToList();
        if (companies is not null)
        {
            foreach (var company in companies)
            {
                companiesList.Add(company.Id,company.Name);
            };
        }

        Dictionary<int, string> usersList = new Dictionary<int, string>();

        var users = userService.GetAllAsync().Result.Data.ToList();
        if (users is not null)
        {
            foreach (var user in users)
            {
                string fullName = $"{ConvertToStandart.ConvertFirstToUpper(user.FirstName)} " +
                    $"{ConvertToStandart.ConvertFirstToUpper(user.LastName)}";
                usersList.Add(user.Id, fullName);
            };
        }

        List<ItemPartnerSale> items = new List<ItemPartnerSale>();
        var cards = (await cardService.GetDeletedAllSimByIdAsync(CompanyId)).Data.ToList();

        var endClock = $"{dpFinishDate.SelectedDate.Value.ToString().Substring(0, 10)} 11:59:59 PM";
        var endDay= DateTime.Parse(endClock);

        
        int ConnectedNumberCount = 0;
        foreach (var card in cards.ToList())
        {

            if (dpInitialDate.SelectedDate <= card.SoldTime && endDay >= card.SoldTime)
            {
                ConnectedNumberCount++;
                items.Add(new ItemPartnerSale
                {
                    Id = ConnectedNumberCount,
                    CompanyName = ConvertToStandart.ConvertFirstToUpper(companiesList[CompanyId]),
                    PartnerFullName = usersList[card.UserId],
                    ConnectedPersonFullName = card.Comment,
                    TariffPlan = card.TariffPlan,
                    ConnectedPhoneNumber = card.ConnectedPhoneNumber,
                    SeriaNumber = card.CardNumber.ToString(),
                    SaleDate = card.SoldTime
                });
            }
        }
        
        return items;
    }

    private void cbCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var selectedValue = cbCompany.SelectedIndex;
        List<ItemComboBox> items = GetItemsFromDatabase();
        CompanyId = items[selectedValue].Id;

        if (selectedValue.Equals(0))
        {
            spReportDate.Visibility = Visibility.Collapsed;
            dataGrid.Visibility = Visibility.Collapsed;
        }
        else
        { 
            spReportDate.Visibility= Visibility.Visible;
        }
    }

    private void btnGetReport_Click(object sender, RoutedEventArgs e)
    {
        if (dpInitialDate.SelectedDate is null || dpFinishDate.SelectedDate is null)
        {
            MessageBox.Show("Sanalarni to'liq kiriting.");
            return;
        }
        else if (dpInitialDate.SelectedDate.Value > dpFinishDate.SelectedDate.Value)
        {
            MessageBox.Show("Boshlangich sana tugash sanasidna katta bo'lmasligi kerak.");
            return;
        }
        else if (dpInitialDate.SelectedDate.Value > DateTime.Now)
        {
            MessageBox.Show("Boshlangich sana bugungi sanadan yuqori bo'lmasligi kerak.");
            return;
        }

            dataGrid.Visibility = Visibility.Visible;
        
        LoadDataAsync();
    }

    private void dpInitialDate_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }
}
