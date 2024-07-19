using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Pages.Companies;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using SimReport.Windows.Companies;
using SimReport.Companents.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Pages
{
    /// <summary>
    /// Interaction logic for CompaniesPage.xaml
    /// </summary>
    public partial class CompaniesPage : Page
    {
        private readonly ICompanyService companyService;
        private IServiceProvider services;

        public CompaniesPage(IServiceProvider services)
        {
            InitializeComponent();
            this.companyService = services.GetRequiredService<ICompanyService>();
            this.services = services;

            Loading();
        }
        public void Loading()
        {
            // Retrieve items from the database
            List<WrapItem> items = GetItemsFromDatabase();

            wrpCompanies.Children.Clear();
            // Create UI elements for each item and add them to the WrapPanel
            foreach (var item in items)
            {
                // Create a CompanyViewUserControl for each item
                CompanyViewUserControl companyViewUserControl = new CompanyViewUserControl(services);

                companyViewUserControl.lbName.Content = item.Name;

                // Add the CompanyViewUserControl to the WrapPanel
                wrpCompanies.Children.Add(companyViewUserControl);
            }
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CompanyCreateWindow companyCreateWindow = new CompanyCreateWindow(services);
            companyCreateWindow.ShowDialog();
            if(companyCreateWindow.IsCraeted)
                Loading();
        }

        public List<WrapItem> GetItemsFromDatabase()
        {
            List<WrapItem> items = new List<WrapItem>();
            var companies = companyService.GetAllAsync().Result.Data.ToList();
            if (companies is not null)
            {
                foreach (var company in companies)
                {
                    items.Add(new WrapItem()
                    {
                        Id = company.Id,
                        Name = ConvertToStandart.ConvertFirstToUpper(company.Name),
                    });
                };
            }
            return items;
        }

        private async void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<WrapItem> items = new List<WrapItem>();
            var companies = companyService.GetAllAsync().Result.Data.ToList();
            if (companies is not null)
            {
                wrpCompanies.Children.Clear();
                foreach (var company in companies)
                {
                    if (company.Name.Contains(tbSearch.Text.ToLower()))

                        items.Add(new WrapItem()
                        {
                            Id = company.Id,
                            Name = ConvertToStandart.ConvertFirstToUpper(company.Name)
                        });
                }

                foreach (var item in items)
                {
                    // Create a CompanyViewUserControl for each item
                    CompanyViewUserControl companyViewUserControl = new CompanyViewUserControl(services);

                    companyViewUserControl.lbName.Content = item.Name;

                    // Add the CompanyViewUserControl to the WrapPanel
                    wrpCompanies.Children.Add(companyViewUserControl);
                }
            }
        }
    }
}
