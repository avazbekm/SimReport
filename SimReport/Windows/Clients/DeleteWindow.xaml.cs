using System;
using System.Windows;
using SimReport.Interfaces;
using System.Threading.Tasks;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows;

/// <summary>
/// Interaction logic for DeleteWindow.xaml
/// </summary>
public partial class DeleteWindow : Window
{
    private readonly IServiceProvider services;

    public DeleteWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;

    }

    private void btnNo_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void btnYes_Click(object sender, RoutedEventArgs e)
    {
        var result = await this.services.GetRequiredService<IUserService>().DeleteAsync(UserPhone.Id);
        if (result.StatusCode.Equals(200))
            MessageBox.Show($"{UserPhone.FirstName} o'chirildi.");
        else
            MessageBox.Show($"{result.Message}");
        
        this.Close();
    }
}
