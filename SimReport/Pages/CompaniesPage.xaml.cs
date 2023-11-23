using SimReport.Windows.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimReport.Pages
{
    /// <summary>
    /// Interaction logic for CompaniesPage.xaml
    /// </summary>
    public partial class CompaniesPage : Page
    {
        public CompaniesPage()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CompanyCreateWindow companyCreateWindow = new CompanyCreateWindow();
            companyCreateWindow.ShowDialog();
        }

        private void CompanyViewUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
