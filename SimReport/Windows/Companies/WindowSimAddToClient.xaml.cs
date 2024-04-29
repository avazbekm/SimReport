﻿using System;
using System.Linq;
using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Cards;
using System.Collections.Generic;
using SimReport.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;


namespace SimReport.Windows.Companies;

/// <summary>
/// Interaction logic for WindowSimAddToClient.xaml
/// </summary>
public partial class WindowSimAddToClient : Window
{
    private readonly ICompanyService companyService;
    private readonly ICardService cardService;

    private int CompanyId;
    public WindowSimAddToClient(IServiceProvider services)
    {
        InitializeComponent();
        this.companyService = services.GetRequiredService<ICompanyService>();
        this.cardService = services.GetRequiredService<ICardService>();


        // Databazadan malumotlarni olish
        List<ItemComboBox> items = GetItemsFromDatabase();

        // Comboboxga malumotlarni yuklash
        cbCompany.ItemsSource = items;
    }

    private void cbCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var selectedValue = cbCompany.SelectedIndex;
        if (selectedValue.Equals(0))
        {
            spAddSimcard.Visibility = Visibility.Collapsed;
        }
        else
        {
            spAddSimcard.Visibility = Visibility.Visible;
            List<ItemComboBox> items = GetItemsFromDatabase();
            CompanyId = items[selectedValue].Id;
        }

    }
    private List<ItemComboBox> GetItemsFromDatabase()
    {
        List<ItemComboBox> items = new List<ItemComboBox>();
        items.Add(new ItemComboBox { Name = "Kompaniya tanlash" });
        var companies = companyService.GetAllAsync().Result.Data.ToList();
        if (companies is not null)
        {
            foreach (var company in companies)
            {
                items.Add(new ItemComboBox()
                {
                    Id = company.Id,
                    Name = ConvertToStandart.ConvertFirstToUpper(company.Name),
                });
            };
        }
        return items;
    }

    private void tbSimcardSeria_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string text = textBox.Text;

        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character))
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                textBox.Text = text.Replace(character.ToString(), "");
                textBox.CaretIndex = text.Length;
                return;
            }
        }
    }

    private void tbSimcardQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string text = textBox.Text;

        // Faqatgina raqamlar kiritilishi mumkin
        foreach (char character in text)
        {
            if (!char.IsDigit(character))
            {
                // Agar raqamdan boshqa belgi kiritilsa o'chirlad
                textBox.Text = text.Replace(character.ToString(), "");
                textBox.CaretIndex = text.Length;
                return;
            }
        }
    }

    private async void btnSave_Click(object sender, RoutedEventArgs e)
    {
        long lastSeria = long.Parse(tbToSimcardSeria.Text);
        long firstSeria = long.Parse(tbSimcardSeria.Text);

        Card card = new Card();

        string cards = "";
        card.UserId = UserPhone.Id;
        card.CompanyId = CompanyId;

        if (firstSeria > lastSeria)
        {
            MessageBox.Show("Oxirgi seria birinchi seriadan katta bo'lmasligi kerak! Etiborli bo'ling.");
            return;
        }

        for (long i = firstSeria; i <= lastSeria; i++) 
        {
            card.CardNumber = i;
            var result = await this.cardService.AddAsync(card);
            card.Id = 0;

            if (!result.StatusCode.Equals(200))
            {
                cards += $"{card.CardNumber}\n";
            }
        }

        if (cards != "")
            MessageBox.Show($" Bu serialar boshqa hamkorga biriktirilgan\n\n{cards}");
        else
            MessageBox.Show($" Biriktirildi."); 
    }
}
