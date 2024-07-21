using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports.Companies
{
    /// <summary>
    /// Interaction logic for CompanyReportWindow.xaml
    /// </summary>
    public partial class CompanyReportWindow : Window
    {
        private readonly ICompanyService companyService;
        private readonly ICardService cardService;

        public CompanyReportWindow(IServiceProvider services)
        {
            InitializeComponent();
            this.companyService = services.GetRequiredService<ICompanyService>();
            this.cardService = services.GetRequiredService<ICardService>();

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                dataGrid.ItemsSource = await GetAllCompaniesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async Task<List<ItemCompany>> GetAllCompaniesAsync()
        {
            List<ItemCompany> items = new List<ItemCompany>();

            // Retrieve all companies
            var companies = await companyService.GetAllAsync();
            if (companies?.Data != null)
            {
                int mobiuz = 0;
                int beeline = 0;
                int ucell = 0;
                int uzmobile = 0;
                int humans = 0;

                var companiesList = companies.Data.ToList();

                foreach (var company in companiesList)
                {
                    var cards = await cardService.GetAllAsync(company.Id);
                    var cardList = cards?.Data;

                    if (cardList != null)
                    {
                        switch (company.Name.ToLower())
                        {
                            case "mobiuz":
                                mobiuz = cardList.Count();
                                break;
                            case "beeline":
                                beeline = cardList.Count();
                                break;
                            case "ucell":
                                ucell = cardList.Count();
                                break;
                            case "uzmobayl":
                                uzmobile = cardList.Count();
                                break;
                            case "humans":
                                humans = cardList.Count();
                                break;
                        }
                    }
                }

                items.Add(new ItemCompany
                {
                    Mobiuz = mobiuz,
                    Beeline = beeline,
                    Ucell = ucell,
                    Uzmobile = uzmobile,
                    Humans = humans,
                    Total = mobiuz + beeline + ucell + uzmobile + humans
                });
            }
            return items;
        }
    }
}
