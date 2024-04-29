using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows
{
    /// <summary>
    /// Interaction logic for ReturnSimcardWindow.xaml
    /// </summary>
    public partial class ReturnSimcardWindow : Window
    {
        private readonly ICompanyService companyService;
        private readonly ICardService cardService;

        private int CompanyId;
        public ReturnSimcardWindow(IServiceProvider services)
        {
            InitializeComponent();
            this.companyService = services.GetRequiredService<ICompanyService>();
            this.cardService = services.GetRequiredService<ICardService>();

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

        private async void cbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = cbCompany.SelectedIndex;
            if (selectedValue.Equals(0))
            {
                svScroll.Visibility = Visibility.Collapsed;
                spCounter.Visibility = Visibility.Collapsed;
            }
            else if (!selectedValue.Equals(0))
            {
                svScroll.Visibility = Visibility.Visible;
                spCounter.Visibility = Visibility.Visible;

                List<ItemComboBox> items = GetItemsFromDatabase();
               
                CompanyId = items[selectedValue].Id;

                List<ItemReturn> itemReturns = new List<ItemReturn>();
                
                var cards = (await this.cardService.GetAllAsync(CompanyId)).Data.ToList();
                if (cards is not null)
                {

                    foreach (var card in cards.Where(card => card.UserId.Equals(UserPhone.Id)))
                    {
                        itemReturns.Add(new ItemReturn()
                        {
                            Id = card.Id,
                            UserId = card.UserId,
                            CompanyId = card.CompanyId,
                            SeriaNumber = card.CardNumber
                        });
                    }

                    dataGrid.ItemsSource = itemReturns;

                    // umumiy sonini chiqarish uchun
                    tbTotalCount.Text = itemReturns.Count.ToString();
                }
            }
        }

        List<(long, int)> selectedSeriaNumbers = new List<(long, int)>();
        private void chbSeriaSelect_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            // Iterate through the selected items in the DataGrid
            foreach (var selectedItem in dataGrid.SelectedItems)
            {
                if (checkBox.IsChecked == true)
                {
                    // You may need to adjust this index based on your actual column layout
                    long seriaNumber = ((ItemReturn)selectedItem).SeriaNumber;
                    int id = ((ItemReturn)selectedItem).CompanyId;
                    selectedSeriaNumbers.Add((seriaNumber, id));
                }
                else
                {
                    long seriaNumber = ((ItemReturn)selectedItem).SeriaNumber;
                    int id = ((ItemReturn)selectedItem).CompanyId;
                    selectedSeriaNumbers.Remove((seriaNumber,id));
                }
                // tanlangalar sonini bilish
                tbCount.Text = selectedSeriaNumbers.Count.ToString();
            }

        }
        private async void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            string comment = tbReturnComment.Text;
           
            string seriaNumbers = "";
            
            foreach (var card in selectedSeriaNumbers)
            {
                var result = await this.cardService.ReturnAsync(card.Item1, card.Item2, comment);

                if (!result.StatusCode.Equals(200))
                    seriaNumbers +=$"{card.Item1.ToString()} \n";
            }

            if (seriaNumbers.Length > 0)
                MessageBox.Show($" Qaytmagan sim kartalar ro'yxati\n qaytadan harakat qilib ko'ring.\n\n{seriaNumbers}");
            else
                MessageBox.Show("Qaytarildi.");
            
            tbCount.Text = "0";
            selectedSeriaNumbers.Clear(); 
        }
    }
}
