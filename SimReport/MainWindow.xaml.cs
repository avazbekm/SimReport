using Microsoft.Extensions.DependencyInjection;
using SimReport.Entities.Users;
using SimReport.Interfaces;
using SimReport.Pages;
using SimReport.Pages.Report;
using SimReport.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SimReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider services;
        public MainWindow(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void brDragable_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else this.WindowState = WindowState.Maximized;
        }

        private void rbClients_Click(object sender, RoutedEventArgs e)
        {

            ClientsPage clientsPage = new ClientsPage(services) ;
            PageNavigator.Content = clientsPage;
        }

        private void rbCompanies_Click(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(services);
            PageNavigator.Content = companiesPage;

        }

        private void rbAbout_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage reportsPage = new ReportsPage(services);
            PageNavigator.Content = reportsPage;
        }

        private void rbAbout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
