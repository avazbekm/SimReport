using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ClosedXML.Excel;
using System.IO;
using System.Windows.Controls;

namespace SimReport.Windows.Reports.Partners;

/// <summary>
/// Interaction logic for PartnerWithSeriaNumber.xaml
/// </summary>
public partial class PartnerWithSeriaNumber : Window
{
    private readonly IUserService userService;
    private readonly ICardService cardService;
    private readonly ICompanyService companyService;

    public PartnerWithSeriaNumber(IServiceProvider services)
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
            dataGrid.ItemsSource = await GetAllCardsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    private async Task<List<ItemPartnerSeriaNumber>> GetAllCardsAsync()
    {
        Dictionary<int, string> companiesList = new Dictionary<int, string>();
        var companies = companyService.GetAllAsync().Result.Data.ToList();
        if (companies is not null)
        {
            foreach (var company in companies)
            {
                companiesList.Add(company.Id, company.Name);
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

        List<ItemPartnerSeriaNumber> items = new List<ItemPartnerSeriaNumber>();
        var cards = (await cardService.GetAllAsync()).Data.ToList();

        int ConnectedNumberCount = 0;
        if (cards is not null)
        {
            foreach (var card in cards)
            {

                ConnectedNumberCount++;
                items.Add(new ItemPartnerSeriaNumber
                {
                    Id = ConnectedNumberCount,
                    PartnerFullName = usersList[card.UserId],
                    CompanyName = ConvertToStandart.ConvertFirstToUpper(companiesList[card.CompanyId]),
                    SeriaNumber = card.CardNumber.ToString(),
                    ComeDate = card.CardsArrivedDate
                });
            }
        }
        return items;
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
                worksheet.Cell(1, 1).Value = $"{DateTime.Now.ToString().Substring(0, 10)}"; ; // Your custom row
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

    private void btnExportSeriaNumber_Click(object sender, RoutedEventArgs e)
    {
        // Export DataGrid to Excel using ClosedXML
        ExportToExcel(dataGrid);
    }
}
