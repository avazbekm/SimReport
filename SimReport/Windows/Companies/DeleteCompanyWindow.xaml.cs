using System;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Pages.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Companies;

/// <summary>
/// Interaction logic for DeleteCompanyWindow.xaml
/// </summary>
public partial class DeleteCompanyWindow : Window
{
    private readonly IServiceProvider services;

    public DeleteCompanyWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private async void btnYes_Click(object sender, RoutedEventArgs e)
    {
        var kompaniya = await services.GetRequiredService<ICompanyService>().GetAsync(CompanyGetId.Name.ToLower());

        int companyId = kompaniya.Data.Id;

        var result = await services.GetRequiredService<ICompanyService>().DeleteAsync(companyId);
        if (result.StatusCode.Equals(200))
            MessageBox.Show("O'chirildi.");
        else
            MessageBox.Show(result.Message);

        this.Close();
    }

    private void btnNo_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
