﻿using SimReport.Interfaces;
using SimReport.Pages;
using System.Windows;

namespace SimReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IUserService userService;

        public MainWindow()
        {
            InitializeComponent();
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

        private void rbDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbClients_Click(object sender, RoutedEventArgs e)
        {
            ClientsPage clientsPage = new ClientsPage(userService);
            PageNavigator.Content = clientsPage;
        }

        private void rbClients_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void rbCompanies_Click(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage();
            PageNavigator.Content = companiesPage;

        }

        private void rbReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbReports_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rbAbout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
