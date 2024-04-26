using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Users;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

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

        if (user.FirstName.Equals("") ||
           user.LastName.Equals("") ||
           user.Phone.Equals(""))
            MessageBox.Show("Malumotni to'liq kiriting!");
        else
        {
            var result = await this.userService.AddAsync(user);

            if (result.StatusCode.Equals(200))
                MessageBox.Show($" Saqlandi.");
            else
                MessageBox.Show($"{result.Message}");
        }
    }

    private void tbPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string text = textBox.Text;

        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character) && character!=' ')
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                textBox.Text = text.Replace(character.ToString(), "");
                textBox.CaretIndex = text.Length;
                return;
            }
        }
    }
}
