using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using SimReport.Entities.Users;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Clients;

/// <summary>
/// Interaction logic for UserEditWindow1.xaml
/// </summary>
public partial class UserEditWindow : Window
{
    private readonly IServiceProvider services;
    public UserEditWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private void tbFirstName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }

    private async void bntSave_Click(object sender, RoutedEventArgs e)
    {
        User user = new User();

        var card = (await services.GetRequiredService<ICardService>().GetAllAsync())
                        .Data
                        .ToList()
                        .FirstOrDefault(u => u.UserId.Equals(UserPhone.Id));
        if (card is not null)
        {
            var existUser = await this.services.GetRequiredService<IUserService>().GetAsync(UserPhone.Id);

            user.Id = UserPhone.Id;
            user.FirstName = existUser.Data.FirstName;
            user.LastName = existUser.Data.LastName;
            user.Phone = tbPhone.Text;

            if (user.Phone.Equals(""))
                MessageBox.Show("Malumotni to'liq kiriting!");
            else
            {
                var result = await this.services.GetRequiredService<IUserService>().UpdateAsync(user);

                if (result.StatusCode.Equals(200))
                    MessageBox.Show($" O'zgardi.");
                else
                    MessageBox.Show($"{result.Message}");
            }
            this.Close();
        }
        else
        {
            user.Id = UserPhone.Id;
            user.FirstName = tbFirstName.Text.ToLower();
            user.LastName = tbLastName.Text.ToLower();
            user.Phone = tbPhone.Text;

            if (user.FirstName.Equals("") ||
                user.LastName.Equals("") ||
                user.Phone.Equals(""))
                MessageBox.Show("Malumotni to'liq kiriting!");
            else
            {
                var result = await this.services.GetRequiredService<IUserService>().UpdateAsync(user);

                if (result.StatusCode.Equals(200))
                    MessageBox.Show($" O'zgardi.");
                else
                    MessageBox.Show($"{result.Message}");
            }
            this.Close();
        }
    }

    private void tbPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string text = tbPhone.Text;

        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character) && character != ' ')
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                tbPhone.Text = text.Replace(character.ToString(), "");
                tbPhone.CaretIndex = text.Length;
                return;
            }
        }

    }
}
