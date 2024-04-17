using System;
using System.Windows.Input;
using System.Windows.Controls;
using SimReport.Pages.Companies;
using SimReport.Windows.Companies;

namespace SimReport.Companents.Companies
{
    /// <summary>
    /// Interaction logic for CompanyViewUserControl.xaml
    /// </summary>
    public partial class CompanyViewUserControl : UserControl
    {
        private readonly IServiceProvider services;
        public CompanyViewUserControl(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;

        }

        private void lbName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CompanyGetId.Name = lbName.Content.ToString();
            CompanyViewEdit companyViewEdit = new CompanyViewEdit(services);
            companyViewEdit.Show();
        }
    }
}
