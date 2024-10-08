﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using SimReport.Interfaces;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        btnExportToExcel.Visibility = Visibility.Visible;


        LoadDataAsync();
    }

    private void ExportToExcel(DataGrid dataGrid)
    {
        try
        {
            // Create a new workbook and worksheet using ClosedXML
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 1. Write the custom row before the headers
                worksheet.Cell(1, 1).Value = $"{dpInitialDate.SelectedDate.Value.ToString().Substring(0, 10)} dan " +
                    $"{dpFinishDate.SelectedDate.Value.ToString().Substring(0, 10)} gacha"; ; // Your custom row
                worksheet.Range(1, 1, 1, dataGrid.Columns.Count).Merge(); // Merge the row across all columns
                worksheet.Row(1).Style.Font.Bold = true; // Optionally, make the text bold
                worksheet.Row(2).Style.Font.Bold = true; // Optionally, make the text bold

                // 2. Add headers from DataGrid to Excel (starting from row 2)
                for (int col = 0; col < dataGrid.Columns.Count; col++)
                {
                    worksheet.Cell(2, col + 1).Value = dataGrid.Columns[col].Header.ToString();
                }

                    // 3. Add rows from DataGrid to Excel (starting from row 3)
                    var itemsSource = dataGrid.ItemsSource as IEnumerable<dynamic>;
                    int row = 3; // Start from the third row (1st row is custom, 2nd row is headers)
                try
                {
                    foreach (var item in itemsSource)
                    {
                        for (int col = 0; col < dataGrid.Columns.Count; col++)
                        {
                            var cellValue = dataGrid.Columns[col].GetCellContent(item) as TextBlock;
                            worksheet.Cell(row, col + 1).Value = cellValue?.Text;
                        }
                        row++;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                // 4. Set column width to adjust to content
                worksheet.Columns().AdjustToContents();

                // 5. Save the file using SaveFileDialog
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xls";
                saveFileDialog.Title = "Excel fayllarni saqlash";
                saveFileDialog.FileName = "SotuvHisobot.xls";

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        workbook.SaveAs(stream);
                        MessageBox.Show("Excel fayl tayyor!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
    {
        // Export DataGrid to Excel using ClosedXML
        ExportToExcel(dataGrid);
    }
}
