using System;
using System.Windows;
using SimReport.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports.Companies;

/// <summary>
/// Interaction logic for CompanyReportWindow.xaml
/// </summary>
public partial class CompanyReportWindow : Window
{
    private readonly ICompanyService companyService;
    public CompanyReportWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.companyService = services.GetRequiredService<ICompanyService>();
    }

}
