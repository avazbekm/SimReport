using System;
using Spire.Pdf;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using SimReport.Interfaces;
using System.Windows.Controls;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Pages.Report;

/// <summary>
/// Interaction logic for ReportsPage.xaml
/// </summary>
public partial class ReportsPage : Page
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;
    int CompanyId;
    string CompanyName;
    public ReportsPage(IServiceProvider services)
    {
        InitializeComponent();
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
        openFileDialog.Filter = "Image files(*.pdf)|*.pdf";
        if (openFileDialog.ShowDialog() == true)
            tbMobiuzAdress.Text = openFileDialog.FileName;
    }

    private async void btnBajarish_Click(object sender, RoutedEventArgs e)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (!tbMobiuzAdress.Text.Equals("  ... ni bosib PDF faylni yuklang"))
        {
            // for reading pdf files 
            PdfDocument document = new PdfDocument();
            document.LoadFromFile(tbMobiuzAdress.Text);

            foreach (PdfPageBase page in document.Pages)
            {
                stringBuilder.Append(page.ExtractText());
            }

            string data = "";
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
                        break;
                    }

                case "Beeline":
                    {
                        break;
                    }

                case "Uzmobayl":
                    {
                        break;
                    }

                case "Humans":
                    {
                        break;
                    }
            }
        }
        else
            MessageBox.Show("PDF faylni yuklang!");
    }
}

