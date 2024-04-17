using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Users;
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

        user.FirstName = tbFirstName.Text.ToLower();
        user.LastName = tbLastName.Text.ToLower();
        user.Phone = tbPhone.Text;

        var result = await this.userService.AddAsync(user);

        if (result.StatusCode.Equals(200))
            MessageBox.Show($" Saqlandi.");
        else
            MessageBox.Show($"{result.Message}");
    }

    private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
