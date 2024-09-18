using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using ExcelDataReader;
using Microsoft.Win32;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Cards;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;


namespace SimReport.Windows.Companies;

/// <summary>
/// Interaction logic for WindowSimAddToClient.xaml
/// </summary>
public partial class WindowSimAddToClient : Window
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;

    private int CompanyId;
    private string CompanyName = "";
    public WindowSimAddToClient(IServiceProvider services)
    {
        InitializeComponent();
        this.companyService = services.GetRequiredService<ICompanyService>();
        this.cardService = services.GetRequiredService<ICardService>();


        // Databazadan malumotlarni olish
        List<ItemComboBox> items = GetItemsFromDatabase();

        // Comboboxga malumotlarni yuklash
        cbCompany.ItemsSource = items;
    }

    private void cbCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var selectedValue = cbCompany.SelectedIndex;
        if (selectedValue.Equals(0))
        {
            spAddSimcard.Visibility = Visibility.Collapsed;
        }
        else
        {
            spAddSimcard.Visibility = Visibility.Visible;
            List<ItemComboBox> items = GetItemsFromDatabase();
            CompanyId = items[selectedValue].Id;
            CompanyName = items[selectedValue].Name;
            if (CompanyName.Equals("Ucell") || CompanyName.Equals("Uzmobile"))
            {
                tbSimcardSeria.MaxLength = 18;
                tbToSimcardSeria.MaxLength = 18;
                spBeelineTablo.Visibility= Visibility.Collapsed;
                spAddSimcard.Visibility = Visibility.Visible;
            }
            else if (CompanyName.Equals("Beeline") &&
                UserPhone.FirstName.Equals("Asosiy") &&
                UserPhone.LastName.Equals("Baza"))
            {
                spBeelineTablo.Visibility = Visibility.Visible;
                spAddSimcard.Visibility = Visibility.Collapsed;
            }
            else if (CompanyName.Equals("Beeline") &&
                !UserPhone.FirstName.Equals("Asosiy") &&
                !UserPhone.LastName.Equals("Baza"))
            {
                tbSimcardSeria.MaxLength = 18;
                tbToSimcardSeria.MaxLength = 18;
                spBeelineTablo.Visibility = Visibility.Collapsed;
                spAddSimcard.Visibility = Visibility.Visible;
            }
            else
            {
                tbSimcardSeria.MaxLength = 19;
                tbToSimcardSeria.MaxLength = 19;
                spBeelineTablo.Visibility = Visibility.Collapsed;
                spAddSimcard.Visibility = Visibility.Visible;
            }
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

    private void tbSimcardSeria_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string text = tbSimcardSeria.Text;
        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character))
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                tbSimcardSeria.Text = text.Replace(character.ToString(), "");
                tbSimcardSeria.CaretIndex = text.Length;
                return;
            }
        }
        if (tbSimcardSeria.Text.Length > 16) 
            tbToSimcardSeria.Text = tbSimcardSeria.Text.Substring(0, 16);
        else
            tbToSimcardSeria.Text = tbSimcardSeria.Text;
    }

    private async void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if ((tbToSimcardSeria.Text.Length < 18 && tbSimcardSeria.Text.Length < 18) &&
            (CompanyName.Equals("Beeline") || CompanyName.Equals("Ucell") || CompanyName.Equals("Uzmobile")))
        {
            MessageBox.Show("Serilalarni to'liq kiriting!");
            return;
        }
        else if ((tbToSimcardSeria.Text.Length < 19 && tbSimcardSeria.Text.Length < 19) &&
            (CompanyName.Equals("Mobiuz")))
        {
            MessageBox.Show("Serilalarni to'liq kiriting!");
            return;
        }

        long lastSeria = long.Parse(tbToSimcardSeria.Text);
        long firstSeria = long.Parse(tbSimcardSeria.Text);

        Card card = new Card();

        string cards = "";
        card.UserId = UserPhone.Id;
        card.CompanyId = CompanyId;

        if (firstSeria > lastSeria)
        {
            MessageBox.Show("Oxirgi seria birinchi seriadan katta bo'lishligi kerak. Etiborli bo'ling!");
            return;
        }
        // asosiy bazaga qo'shish
        if (UserPhone.FirstName.Equals("Asosiy") && UserPhone.LastName.Equals("Baza"))
        {
            for (long i = firstSeria; i <= lastSeria; i++)
            {
                card.Id = 0;
                card.CardNumber = i;
                card.CardsArrivedDate = DateTime.UtcNow;
                var result = await this.cardService.AddAsync(card);

                if (!result.StatusCode.Equals(200))
                {
                    cards += $"{card.CardNumber}\n";
                }
            }

            if (cards != "")
                MessageBox.Show($" Bu serialar boshqa hamkorga biriktirilgan\n\n{cards}");
            else
                MessageBox.Show($" Biriktirildi.");
            this.Close();
        }
        else
        {
            List<string> list = new List<string>();
            // kompaniya id si va asosiy baza nomi orqali hamma asosiy bazaga tegishli cardlarni olamiz
            var cardlar = (await this.cardService.GetAllAsync(CompanyId,"asosiy","baza")).Data;
            if (cardlar is not null) 
            {
                for (long i = firstSeria; i <= lastSeria; i++)
                {
                    int k = 0;
                    foreach (var kart in cardlar)
                        if (kart.CardNumber == i)
                        {
                            // asosiy bazadan o'chirish uchun jamlanmoqda
                            list.Add(kart.Id.ToString());
                            // hamkorga o'tkazish uchun
                            kart.Id = 0;
                            kart.UserId = UserPhone.Id;
                            var result = await this.cardService.TransferAsync(kart);

                            k++;

                            if (!result.StatusCode.Equals(200))
                                cards += $"{i}\n";
                            break;
                        }

                    if (k == 0)
                        cards += $"{i}\n";
                }
                //hamkorga o'tkazib bo'lingan cardlarni asosiy bazadan o'chiramiz
                foreach (var kart in list)
                {
                    await this.cardService.DeleteAsync(int.Parse(kart), UserPhone.FirstName, UserPhone.LastName);
                }

                if (cards != "")
                    MessageBox.Show($" Bu serialar asosiy bazada mavjud emas! \n\n{cards}");
                else
                    MessageBox.Show($" Biriktirildi.");
                this.Close();
            }
            else
            {
                for (long i = firstSeria; i <= lastSeria; i++)
                    cards += $"{i}\n";
                MessageBox.Show($"Bu serialar asosiy bazada mavjud emas.\n\n{cards}");
                this.Close();
            }
        }
    }

    private void tbToSimcardSeria_TextChanged(object sender, TextChangedEventArgs e)
    {

        //tbToSimcardSeria.Text = tbSimcardSeria.Text.Substring(0, 17);
        string text = tbToSimcardSeria.Text;

        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character))
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                tbToSimcardSeria.Text = text.Replace(character.ToString(), "");
                tbToSimcardSeria.CaretIndex = text.Length;
                return;
            }
        }

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files(*.xls;*.xlsx)|*.xls;*.xlsx";
        if (openFileDialog.ShowDialog() == true)
            tbBeelineWay.Text = openFileDialog.FileName;
    }

    private async void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        if (tbBeelineWay.Text.Equals("  ... ni bosib excel faylni yuklang"))
        {
            MessageBox.Show("Excel faylni yuklang.");
            return;
        }

        DataTable dataTable = new DataTable();
        dataTable = ReadExcelFile(tbBeelineWay.Text);
        
        string phoneNumbers = "";
        Card card = new Card();
        foreach (DataRow row in dataTable.Rows)
        {
            try
            {
                card.Id = 0;
                card.ConnectedPhoneNumber = $"{row[2]}";
                card.CardNumber = long.Parse(row[1].ToString());
                card.UserId = UserPhone.Id;
                card.CompanyId = CompanyId;
                card.CardsArrivedDate = DateTime.UtcNow;
                var result = (await this.cardService.AddAsync(card)).StatusCode;
                if (!result.Equals(200))
                    phoneNumbers += $"{row[1]}\n";
            }
            catch 
            { }
        }

        if (phoneNumbers != "") 
            MessageBox.Show($"Bazada mavjud serialar ro'yxati \n {phoneNumbers}");
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
}
