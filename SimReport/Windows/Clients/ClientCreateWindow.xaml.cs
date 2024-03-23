using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Users;
using SimReport.Services;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Clients;

/// <summary>
/// Interaction logic for ClientCreateWindow.xaml
/// </summary>
public partial class ClientCreateWindow : Window
{
    private readonly IUserService userService;
    public ClientCreateWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.userService = services.GetRequiredService<IUserService>();
    }

    private async void bntSave_Click(object sender, RoutedEventArgs e)
    {
        User user = new User();

        user.FirstName = tbFirstName.Text;
        user.LastName = tbLastName.Text;
        user.Phone = tbPhone.Text;

        var result = await this.userService.AddAsync(user);

        MessageBox.Show($" {result.Message} \n {result.StatusCode}");
    }

    private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
