using SimReport.Entities.Users;
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
using System.Windows.Shapes;

namespace SimReport.Windows.Clients
{
    /// <summary>
    /// Interaction logic for ClientCreateWindow.xaml
    /// </summary>
    public partial class ClientCreateWindow : Window
    {
        public ClientCreateWindow()
        {
            InitializeComponent();
        }

        private void bntSave_Click(object sender, RoutedEventArgs e)
        {
            //var user = new User()
            //{
            //    FirstName = tbFirstName.Text,
            //    LastName = tbLastName.Text,
            //    Phone = tbPhone.Text
            //};


            var FirstName = tbFirstName.Text;
            var LastName = tbLastName.Text;
            var Phone = tbPhone.Text;
            MessageBox.Show($"{FirstName} {LastName} {Phone}");
        }

        private void tbFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
