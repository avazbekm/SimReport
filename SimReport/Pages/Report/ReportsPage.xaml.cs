﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using Microsoft.Win32;
using ExcelDataReader;
using System.Windows.Media;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Windows.Reports;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using SimReport.Windows.Reports.Partners;
using SimReport.Windows.Reports.Companies;
using SimReport.Windows.Reports.BlockWindow;
using SimReport.Windows.Reports.PartnerSales;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Pages.Report;

/// <summary>
/// Interaction logic for ReportsPage.xaml
/// </summary>
public partial class ReportsPage : Page
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;
    private readonly IServiceProvider services;
    private readonly IBlockService blockService;
    int CompanyId;
    string CompanyName;


    public ReportsPage(IServiceProvider services)
    {

        InitializeComponent();
        this.blockService=services.GetRequiredService<IBlockService>();

        var blockDate= blockService.GetAllAsync().Result.Data.ToList()[0];
        if (blockDate.EndDate <= DateTime.UtcNow.Date)
        {
            spReportPage.Visibility = Visibility.Collapsed;
            WindowBlock windowBlock = new WindowBlock(services);
            windowBlock.ShowDialog();
        } 

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
            if (CompanyName.Equals("Mobiuz") || 
                CompanyName.Equals("Uzmobile") || 
                CompanyName.Equals("Beeline") ||
                CompanyName.Equals("Ucell")) 
                tbMobiuzAdress.Text = " ... ni bosib excel file yuklang.";
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files(*.xls;*.xlsx)|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
                tbMobiuzAdress.Text = openFileDialog.FileName;
    }

    private async void btnBajarish_Click(object sender, RoutedEventArgs e)
    {
        //StringBuilder stringBuilder = new StringBuilder();
        DataTable dataTable = new DataTable();
        if (Path.GetExtension(tbMobiuzAdress.Text).Equals(".xls") ||
            Path.GetExtension(tbMobiuzAdress.Text).Equals(".xlsx"))
        {
            dataTable = ReadExcelFile(tbMobiuzAdress.Text);
        }

        try
        {
            switch (CompanyName)
            {
                case "Mobiuz":
                    {
                        // kompaniyaga tegish barcha sim kartalarni olish kerak
                        var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                        if (cards != null)
                        {
                            int quantity = 0;
                            var cardSerias = "";
                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (var card in cards)
                                    if (row[13].ToString().Contains(card.CardNumber.ToString()))
                                    {
                                        card.SoldTime = Convert.ToDateTime(row[11]);
                                        card.TariffPlan = row[7].ToString();
                                        card.ConnectedPhoneNumber = row[12].ToString();
                                        card.Comment = $"{row[2]}";

                                        var result = await this.cardService.SellAsync(card);
                                        if (result.StatusCode.Equals(200))
                                            quantity++;
                                        else
                                            cardSerias += $"{card.CardNumber}\n";
                                        break;
                                    }
                            }
                            MessageBox.Show($"{quantity} ta sotilgan.\n Quyidagi serialar xatolik \n {cardSerias}");
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
                            var cardSerias = "";
                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (var card in cards)
                                    if (row[6].ToString().Contains(card.CardNumber.ToString()))
                                    {
                                        card.SoldTime = Convert.ToDateTime(row[4]);
                                        card.TariffPlan = row[5].ToString();
                                        card.ConnectedPhoneNumber = row[2].ToString();
                                        card.Comment = $"{row[1]}";
                                        var result = await this.cardService.SellAsync(card);
                                        if (result.StatusCode.Equals(200))
                                            quantity++;
                                        else
                                            cardSerias += $"{card.CardNumber}\n";
                                        break;
                                    }
                            }
                            MessageBox.Show($"{quantity} ta sotilgan.\n Quyidagi serialar xatolik \n  {cardSerias}");
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
                        // kompaniyaga tegish barcha sim kartalarni olish kerak
                        var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                        if (cards != null)
                        {
                            int quantity = 0;
                            var cardSerias = "";
                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (var card in cards)
                                    try
                                    {
                                        if (row[1].ToString().Contains(card.ConnectedPhoneNumber.ToString()))
                                        {
                                            card.SoldTime = DateTime.Parse($"{row[6]}");
                                            card.TariffPlan = row[9].ToString();
                                            if (!row[7].ToString().Equals(""))
                                                card.ConnectedPhoneNumber = $"{row[7]}";
                                            card.Comment = $"{row[3]}";

                                            var result = await this.cardService.SellAsync(card);
                                            if (result.StatusCode.Equals(200))
                                                quantity++;
                                            else
                                                cardSerias += $"{card.CardNumber}\n";
                                            break;
                                        }
                                    }
                                    catch { }
                            }
                            MessageBox.Show($"{quantity} ta sotilgan.\n Quyidagi serialar xatolik \n {cardSerias}");
                            break;
                        }
                        else
                        {
                            MessageBox.Show($"{CompanyName} kompaniyaga biriktirilgan sim kartalar mavjud emas.");
                            break;
                        }
                    }

                case "Uzmobile":
                    {
                        // kompaniyaga tegishli barcha sim kartalarni olish kerak
                        var cards = (await this.cardService.GetAllAsync(CompanyId)).Data;
                        if (cards != null)
                        {
                            int quantity = 0;
                            var cardSerias = "";
                            foreach (DataRow row in dataTable.Rows)
                            {
                                foreach (var card in cards)
                                    if (row[11].ToString().Contains(card.CardNumber.ToString()))
                                    {
                                        card.SoldTime = Convert.ToDateTime(row[13]);
                                        card.TariffPlan = row[9].ToString();
                                        card.ConnectedPhoneNumber = row[10].ToString();
                                        card.Comment = $"{row[5]}";

                                        var result = await this.cardService.SellAsync(card);
                                        if (result.StatusCode.Equals(200))
                                            quantity++;
                                        else
                                            cardSerias += $"{card.CardNumber}\n";
                                        break;
                                    }
                            }
                            MessageBox.Show($"{quantity} ta sotilgan.\n Quyidagi serialar xatolik \n {cardSerias}");
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
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
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
                            UseHeaderRow = false
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

    private void lbPartnerSaleReport_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        PartnerSaleWindow partnerSaleWindow = new PartnerSaleWindow(services);
        partnerSaleWindow.ShowDialog();
    }

    private void lbPartnerWithSeriaReport_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        PartnerWithSeriaNumber partnerWithSeriaNumber = new PartnerWithSeriaNumber(services);
        partnerWithSeriaNumber.ShowDialog();
    }

}

