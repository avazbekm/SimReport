using System;
using System.Windows;
using SimReport.Pages;
using SimReport.Pages.Report;
using SimReport.Pages.AboutUs;
using SimReport.Pages.Dashboard;

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

            ClientsPage clientsPage = new ClientsPage(services);
            PageNavigator.Content = clientsPage;
        }

        private void rbCompanies_Click(object sender, RoutedEventArgs e)
        {
            CompaniesPage companiesPage = new CompaniesPage(services);
            PageNavigator.Content = companiesPage;

        }

        private void rbDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardPage dashboardPage = new DashboardPage(services);
            PageNavigator.Content = dashboardPage;
        }

        private void rbReports_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage reportsPage = new ReportsPage(services);
            PageNavigator.Content = reportsPage;
        }

        private void rbAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutPage aboutPage = new AboutPage();
            aboutPage.tbAbout.Content = @$"
                                Assalomu alaykum hurmatli foydalanuvchi! 
             Bu dastur uyali aloqa kompaniya dillerlariga sim karta hisobotni olib yurish
        uchun Siddiqov Avazbek tomonidan yaratildi. O'ylaymanki bu sizga ko'makchi  
        bo'lib, ko'pgina qulayliklar yaratadi va vaqtingizni tejashda yordam beradi." +
                $"\n\n\n\n" +
                $"\tMurojaat uchun telegram username:   @avazbeksm" +
                $"\n\t\t\t\t          tel:   97 334-0-334";  
            PageNavigator.Content = aboutPage;
        }

    }
}
