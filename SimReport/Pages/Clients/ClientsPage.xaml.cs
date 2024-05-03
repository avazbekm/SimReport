using System;
using System.Linq;
using System.Windows;
using SimReport.Windows;
using SimReport.Interfaces;
using SimReport.Pages.Clients;
using System.Windows.Controls;
using SimReport.Windows.Clients;
using SimReport.Services.Helpers;
using System.Collections.Generic;
using SimReport.Windows.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Pages
{
    /// <summary>
    /// Interaction logic for ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        private readonly IServiceProvider services;
        public ClientsPage(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;

            List<Item> items = new List<Item>();
            var users = services.GetRequiredService<IUserService>().GetAllAsync().Result.Data.ToList();
            foreach (var user in users)
            {
                items.Add(new Item()
                {
                    Id = user.Id,
                    Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                    Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                    Phone = user.Phone
                });
            }
            dataGrid.ItemsSource = items;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClientCreateWindow clientCreateWindow = new ClientCreateWindow(services);
            clientCreateWindow.ShowDialog();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Item> items = new List<Item>();
            var users = services.GetRequiredService<IUserService>().GetAllAsync().Result.Data.ToList();
            if (users is not null)
            {
                dataGrid.ItemsSource = string.Empty;
                foreach ( var user in users ) 
                {
                    if (user.FirstName.Contains(tbSearch.Text.ToLower()) ||
                        user.LastName.Contains(tbSearch.Text.ToLower()) ||
                        user.Phone.Contains(tbSearch.Text.ToLower()))

                        items.Add(new Item()
                        {
                            Name = ConvertToStandart.ConvertFirstToUpper(user.FirstName),
                            Surname = ConvertToStandart.ConvertFirstToUpper(user.LastName),
                            Phone = user.Phone
                        });
                }
                dataGrid.ItemsSource=items;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Item selectedUser)
            {
                UserPhone.Id = selectedUser.Id;
                UserPhone.FirstName = selectedUser.Name;
                UserPhone.LastName = selectedUser.Surname;
                // Open a new window passing the selected user's Id as parameter
                WindowSimAddToClient windowSimAddToClient = new WindowSimAddToClient(services);
                windowSimAddToClient.ShowDialog();
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Item selectedUser)
                UserPhone.Id = selectedUser.Id;

            ReturnSimcardWindow returnSimcardWindow = new ReturnSimcardWindow(services);
            returnSimcardWindow.ShowDialog();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Item selectedUser)
                UserPhone.Id = selectedUser.Id;

            UserEditWindow userEditWindow = new UserEditWindow(services);

            var card = (await services.GetRequiredService<ICardService>().GetAllAsync())
                .Data
                .ToList()
                .FirstOrDefault(u => u.UserId.Equals(UserPhone.Id));
            
            if (card is not null)
            {
                MessageBox.Show(@"Bu hamkorga biriktirilgan sim kartalar mavjud faqat telefon nomerini o'zgatirish mumkin.");
                userEditWindow.tbFirstName.Visibility = Visibility.Collapsed;
                userEditWindow.tbLastName.Visibility = Visibility.Collapsed;
                userEditWindow.ShowDialog();
                return;
            }
            userEditWindow.ShowDialog();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Item selectedUser)
            {
                UserPhone.Id = selectedUser.Id;
                UserPhone.FirstName = selectedUser.Name;
                UserPhone.LastName = selectedUser.Surname;
            }

            if(UserPhone.FirstName.Equals("Asosiy") && UserPhone.LastName.Equals("Baza"))
            {
                MessageBox.Show("Asosiy bazani o'chirish mumkin emas!");
                return;
            }
            var card = (await services.GetRequiredService<ICardService>().GetAllAsync())
                .Data
                .ToList()
                .FirstOrDefault(u => u.UserId.Equals(UserPhone.Id));
            if(card is not null)
            {
                MessageBox.Show($"{UserPhone.FirstName}ga sim kartalar biriktirilgan.");
                return;
            }

            DeleteWindow deleteWindow = new DeleteWindow(services);
            deleteWindow.ShowDialog();
        }
    }
}
