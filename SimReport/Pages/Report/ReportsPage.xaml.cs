using System;
using Spire.Pdf;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Windows.Reports;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using SimReport.Windows.Reports.Companies;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Windows.Documents;

namespace SimReport.Pages.Report;

/// <summary>
/// Interaction logic for ReportsPage.xaml
/// </summary>
public partial class ReportsPage : Page
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;
    private readonly IServiceProvider services;
    int CompanyId;
    string CompanyName;

    public ReportsPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        this.companyService = services.GetRequiredService<ICompanyService>();
        this.cardService = services.GetRequiredService<ICardService>();

        // Databazadan malumotlarni olish
        List<ItemComboBox> items = GetItemsFromDatabase();

        // Comboboxga malumotlarni yuklash
        cbCompany.ItemsSource = items;
    }

    private void cbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedValue = cbCompany.SelectedIndex;
        if (selectedValue.Equals(0))
        {
            bName.CornerRadius = new CornerRadius(5);
            spRightSide.Visibility = Visibility.Collapsed;
        }
        else
        {
            bName.CornerRadius = new CornerRadius(5, 0, 0, 5);
            spRightSide.Visibility = Visibility.Visible;
            List<ItemComboBox> items = GetItemsFromDatabase();
            CompanyId = items[selectedValue].Id;
            CompanyName = items[selectedValue].Name;
            if (CompanyName.Equals("Uzmobayl") || CompanyName.Equals("Beeline"))
                tbMobiuzAdress.Text = " ... ni bosib excel file yuklang.";
            else if (CompanyName.Equals("Mobiuz") || CompanyName.Equals("Ucell")) 
                tbMobiuzAdress.Text = " ... ni bosib PDF file yuklang.";
        }
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

    private void btnPDFReader_Click(object sender, RoutedEventArgs e)
    {
        if (tbMobiuzAdress.Text.Equals(" ... ni bosib PDF file yuklang."))
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files(*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
                tbMobiuzAdress.Text = openFileDialog.FileName;
        }
        else if (tbMobiuzAdress.Text.Equals(" ... ni bosib excel file yuklang."))
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files(*.xls;*.xlsx)|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
                tbMobiuzAdress.Text = openFileDialog.FileName;
        }
    }

    private async void btnBajarish_Click(object sender, RoutedEventArgs e)
    {
        StringBuilder stringBuilder = new StringBuilder();
        DataTable dataTable = new DataTable();
        if (Path.GetExtension(tbMobiuzAdress.Text).Equals(".pdf"))
        {
            // for reading pdf files 
            PdfDocument document = new PdfDocument();
            document.LoadFromFile(tbMobiuzAdress.Text);

            foreach (PdfPageBase page in document.Pages)
            {
                stringBuilder.Append(page.ExtractText());
            }
        }
        else if (Path.GetExtension(tbMobiuzAdress.Text).Equals(".xls") || Path.GetExtension(tbMobiuzAdress.Text).Equals(".xlsx"))
        {
            dataTable = ReadExcelFile(tbMobiuzAdress.Text);
        }

        string data = "";
        string comment = "";
        switch (CompanyName)
        {
            case "Mobiuz":
                {
                    // kompaniyaga tegish barcha sim kartalarni olish kerak
                    var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                    if (cards != null)
                    {
                        int quantity = 0;
                        foreach (string row in stringBuilder.ToString().Split("\n"))
                        {
                            if (row.Length < 100)
                                continue;

                            foreach (string piece in row.Split(" "))
                            {
                                if (piece.Equals(""))
                                    continue;
                                if (piece.Length.Equals(10) && piece.IndexOf('.').Equals(2))
                                    data = piece;
                                if (piece.Length.Equals(12) && piece.IndexOf('9').Equals(0))
                                    comment = piece;
                                if (piece.Length == 22 && piece.Contains('-'))
                                {
                                    foreach (var card in cards)
                                    if (piece.Contains(card.CardNumber.ToString()))
                                    {
                                        card.SoldTime = data;
                                        await this.cardService.SellAsync(card);
                                        quantity++;
                                    }
                                }
                            }
                        }
                        MessageBox.Show($"{quantity} ta sotilgan.");
                        break;
                    }
                    else
                    {
                        MessageBox.Show($"{CompanyName} kompaniyaga biriktirilgan sim kartalar mavjud emas.");
                        break;
                    }
                }

            case "Ucell":
                {
                    // kompaniyaga tegish barcha sim kartalarni olish kerak
                    var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                    if (cards != null)
                    {
                        int quantity = 0;
                        foreach (string row in stringBuilder.ToString().Split("\n"))
                        {
                            if (row.Length < 100)
                                continue;

                            foreach (string piece in row.Split(" "))
                            {
                                if (piece.Equals(""))
                                    continue;
                                if (piece.Length.Equals(10) && piece.IndexOf('.').Equals(2))
                                    data = piece;
                                if (piece.Length.Equals(12) && piece.IndexOf('9').Equals(0))
                                    comment = piece;
                                if (piece.Length == 19)
                                {
                                    foreach (var card in cards)
                                        if (piece.Contains(card.CardNumber.ToString()))
                                        {
                                            card.SoldTime = data;
                                            card.Comment = comment;
                                            await this.cardService.SellAsync(card);
                                            quantity++;
                                        }
                                }
                            }
                        }
                        MessageBox.Show($"{quantity} ta sotilgan.");
                        break;
                    }
                    else
                    {
                        MessageBox.Show($"{CompanyName} kompaniyaga biriktirilgan sim kartalar mavjud emas.");
                        break;
                    }
                }

            case "Beeline":
                {
                    List<string> seriaNumbers = new List<string>();
                    // kompaniyaga tegish barcha sim kartalarni olish kerak
                    var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                    if (cards != null)
                    {
                        int quantity = 0;
                        foreach (DataRow row in dataTable.Rows)
                            seriaNumbers.Add(row[4].ToString());


                        foreach (var card in cards)
                        {
                            bool isConnected = true;

                            foreach (var seria in seriaNumbers)
                                if (card.CardNumber.ToString().Contains(seria))
                                    isConnected = false;
                           
                            if(isConnected)
                            {
                                card.Comment = "Ulangan";
                                await this.cardService.SellAsync(card);
                                quantity++;
                            }
                        }

                            MessageBox.Show($"{quantity} ta sotilgan.");
                        break;
                    }
                    else
                    {
                        MessageBox.Show($"{CompanyName} kompaniyaga biriktirilgan sim kartalar mavjud emas.");
                        break;
                    }
                }

            case "Uzmobayl":
                {
                    // kompaniyaga tegish barcha sim kartalarni olish kerak
                    var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                    if (cards != null)
                    {
                        int quantity = 0;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            foreach (var card in cards)
                                if (row[11].ToString().Contains(card.CardNumber.ToString()))
                                { 
                                    card.SoldTime = row[13].ToString().Substring(0,11);
                                    card.Comment = row[10].ToString();
                                    await this.cardService.SellAsync(card);
                                    quantity++;
                                }
                        }
                        MessageBox.Show($"{quantity} ta sotilgan.");
                        break;
                    }
                    else
                    {
                        MessageBox.Show($"{CompanyName} kompaniyaga biriktirilgan sim kartalar mavjud emas.");
                        break;
                    }
                }

            case "Humans":
                {
                    break;
                }
            }
    }

    private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        lbPartnerReport.Foreground = new SolidColorBrush(Colors.White);
        PartnerReportWindow partnerReportWindow = new PartnerReportWindow(services);
        partnerReportWindow.ShowDialog();
    }

    private void lbCompanyReport_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CompanyReportWindow companyReportWindow = new CompanyReportWindow(services);
        companyReportWindow.ShowDialog();
    }

    private DataTable ReadExcelFile(string filePath)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    return result.Tables[0]; // Return the first worksheet
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while reading the Excel file: {ex.Message}");
            return null;
        }
    }

}

