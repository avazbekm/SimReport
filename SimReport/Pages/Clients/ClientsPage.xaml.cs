using Microsoft.Extensions.DependencyInjection;
using SimReport.Interfaces;
using SimReport.Pages.Clients;
using SimReport.Windows.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClientCreateWindow clientCreateWindow = new ClientCreateWindow(services);
            clientCreateWindow.ShowDialog();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
