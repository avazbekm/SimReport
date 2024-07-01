using System;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Pages.Companies;
using SimReport.Entities.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Companies;

/// <summary>
/// Interaction logic for CompanyViewEdit.xaml
/// </summary>
public partial class CompanyViewEdit : Window
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;
    private readonly IServiceProvider services;
    public CompanyViewEdit(IServiceProvider services)
    {
        InitializeComponent();
        this.companyService = services.GetRequiredService<ICompanyService>();
        this.cardService = services.GetRequiredService<ICardService>();
        this.services = services;

        tbComName.Text = CompanyGetId.Name;
    }

    private async void btnEdit_Click(object sender, RoutedEventArgs e)
    {
        var kompaniya = await this.companyService.GetAsync(CompanyGetId.Name.ToLower());

        Company company = new Company();
        company.Id = kompaniya.Data.Id;
        company.Name = tbComName.Text.Trim().ToLower();

        if (!company.Name.Equals(""))
        {
            var cards = (await this.cardService.GetAllAsync(company.Id)).Data;
            if (cards is null)
            {
                var companies = (await this.companyService.GetAllAsync()).Data;
                foreach (var item in companies)
                {
                    if (tbComName.Text.ToLower() == item.Name)
                    {
                        MessageBox.Show("Bu nom bilan kompaniya mavjud.");
                        return;
                    }
                }

                var result = await this.companyService.UpdateAsync(company);
                if (result.StatusCode.Equals(200))
                    MessageBox.Show("O'zgartirildi.");
                else if (result.StatusCode.Equals(403))
                    MessageBox.Show(result.Message);
                else
                    MessageBox.Show(result.Message);
            }
            else
                MessageBox.Show("Bu kompaniyaga biriktirilgan sim kartalar mavjud.");
        }
        else
            MessageBox.Show("Kompaniya nomini kiriting.");
    }

    private void tbComName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }

    private async void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        var kompaniya = await this.companyService.GetAsync(CompanyGetId.Name.ToLower());

        int companyId = kompaniya.Data.Id;
        var cards = await this.cardService.GetAllAsync(companyId);
        
        if (!cards.StatusCode.Equals(200)) 
        {
            DeleteCompanyWindow deleteCompanyWindow = new DeleteCompanyWindow(services);
            deleteCompanyWindow.ShowDialog();
        }
        else
            MessageBox.Show("Bu kompaniyaga biriktirilgan sim kartalar mavjud.");
    }
}
