using SimReport.Entities.Users;
using SimReport.Interfaces;
using SimReport.Repositories;
using SimReport.Services;
using SimReport.Windows.Clients;
using System.Windows;
using System.Windows.Controls;

namespace SimReport.Pages
{
    /// <summary>
    /// Interaction logic for ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        private readonly IUserService userService;

        public ClientsPage(IUserService userService)
        {
            InitializeComponent();
            this.userService = userService;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ClientCreateWindow clientCreateWindow = new ClientCreateWindow(userService);
            clientCreateWindow.ShowDialog();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
