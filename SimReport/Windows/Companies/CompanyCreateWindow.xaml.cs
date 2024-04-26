using System;
using System.IO;
using System.Windows;
using SimReport.Interfaces;
using System.Windows.Controls;
using SimReport.Entities.Companies;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Companies;

/// <summary>
/// Interaction logic for CompanyCreateWindow.xaml
/// </summary>
public partial class CompanyCreateWindow : Window
{
    private readonly ICompanyService companyService;
    public CompanyCreateWindow(IServiceProvider services)
    {
        InitializeComponent();
        this.companyService = services.GetRequiredService<ICompanyService>();
    }

    private void btnImageSelector_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Open file dialog to select an image file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                // Read the selected image file
                string imagePath = openFileDialog.FileName;
                byte[] imageData = File.ReadAllBytes(imagePath);

                // Display the uploaded image
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                uploadedImage.Source = bitmapImage;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void btnSave_Click(object sender, RoutedEventArgs e)
    {
        Company company = new Company();

        company.Name = tbCompanyName.Text.ToLower();

        if (company.Name.Equals(""))
            MessageBox.Show("Malumotni to'liq kiriting.");
        else
        {
            var result = await this.companyService.AddAsync(company);
            if (result.StatusCode.Equals(200))
                MessageBox.Show($"Saqlandi.");
            else
                MessageBox.Show($"{result.Message}");
        }
    }
}
