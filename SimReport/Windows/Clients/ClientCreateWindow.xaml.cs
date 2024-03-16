using System.Windows;
using SimReport.Services;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Users;

namespace SimReport.Windows.Clients;

/// <summary>
/// Interaction logic for ClientCreateWindow.xaml
/// </summary>
public partial class ClientCreateWindow : Window
{
    private readonly IUserService userService;


    public ClientCreateWindow(IUserService userService)
    {
        InitializeComponent();
        this.userService = userService;
    }

    private async void bntSave_Click(object sender, RoutedEventArgs e)
    {
        User user = new User();

        user.FirstName = tbFirstName.Text;
        user.LastName = tbLastName.Text;
        user.Phone = tbPhone.Text;
        
        await this.userService.CreateAsync(user);

        var FirstName = tbFirstName.Text;
        var LastName = tbLastName.Text;
        var Phone = tbPhone.Text;
        MessageBox.Show($"{FirstName} {LastName} {Phone}");
    }

    private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}
