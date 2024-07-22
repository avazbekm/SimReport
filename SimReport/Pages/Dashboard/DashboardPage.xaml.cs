using System;
using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Pages.Dashboard;

/// <summary>
/// Interaction logic for DashboardPage.xaml
/// </summary>
public partial class DashboardPage : Page
{
    private readonly IServiceProvider services;
    public DashboardPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
    }

    private async void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = tbSearch.Text;
        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character))
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                tbSearch.Text = text.Replace(character.ToString(), "");
                tbSearch.CaretIndex = text.Length;
                return;
            }
        }

        if (tbSearch.Text.Length == 18 || tbSearch.Text.Length == 19)
        {
            long cardSeria = long.Parse(tbSearch.Text);
            var card = await services.GetRequiredService<ICardService>().GetAsync(cardSeria);

            if (card.StatusCode.Equals(200)) 
            {
                tbComment.Visibility = Visibility.Collapsed;

                var user = await services.GetRequiredService<IUserService>().GetAsync(card.Data.UserId);
                tbName.Text = $"Ism va famiyasi: {ConvertToStandart.ConvertFirstToUpper(user.Data.FirstName)} " +
                    $"{ConvertToStandart.ConvertFirstToUpper(user.Data.LastName)}";
                tbPhone.Text = $" Telefon nomeri: {user.Data.Phone}";
                tbDate.Text = $"Sana: {card.Data.CreatedAt.ToString().Substring(0,10)}";
            }
            else
            {
                // o'chirilgan sim kartalarni topish uchun
                card = await services.GetRequiredService<ICardService>().GetSimAsync(cardSeria);

                if (card.StatusCode.Equals(200))
                {
                    tbComment.Visibility=Visibility.Visible;

                    var user = await services.GetRequiredService<IUserService>().GetAsync(card.Data.UserId);
                    tbName.Text = $"Ism va famiyasi: {ConvertToStandart.ConvertFirstToUpper(user.Data.FirstName)} " +
                        $"{ConvertToStandart.ConvertFirstToUpper(user.Data.LastName)}";
                    tbPhone.Text = $" Telefon nomeri: {user.Data.Phone}";
                    tbDate.Text = $"Sana: {card.Data.CreatedAt.ToString().Substring(0, 10)}";
                    tbComment.Text = $"Izoh: {card.Data.Comment}";
                }
                else
                    MessageBox.Show("Bunday seriali sim karta mavjud emas.");
            }
        }

    }
}
