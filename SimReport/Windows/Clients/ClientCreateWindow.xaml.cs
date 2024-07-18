﻿using System;
using System.Windows;
using SimReport.Pages;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Users;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Clients;

/// <summary>
/// Interaction logic for ClientCreateWindow.xaml
/// </summary>
public partial class ClientCreateWindow : Window
{
    private readonly IUserService userService;
    public bool IsCreated {  get; set; } = false;
    public ClientCreateWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.userService = services.GetRequiredService<IUserService>();
    }

    private async void bntSave_Click(object sender, RoutedEventArgs e)
    {
        User user = new User();

        user.FirstName = tbFirstName.Text.Trim().ToLower();
        user.LastName = tbLastName.Text.Trim().ToLower();
        user.Phone = tbPhone.Text;

        if (user.FirstName.Equals("") ||
           user.LastName.Equals("") ||
           user.Phone.Equals(""))
            MessageBox.Show("Malumotni to'liq kiriting!");
        else
        {
            var result = await this.userService.AddAsync(user);

            if (result.StatusCode.Equals(200))
            {
                MessageBox.Show($" Saqlandi.");
                IsCreated = true;
            }
            else
                MessageBox.Show($"{result.Message}");
        }
        this.Close();    
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
