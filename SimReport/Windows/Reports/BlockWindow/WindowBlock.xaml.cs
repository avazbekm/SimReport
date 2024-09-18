using System;
using System.Windows;
using SimReport.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SimReport.Windows.Reports.BlockWindow;

/// <summary>
/// Interaction logic for WindowBlock.xaml
/// </summary>
public partial class WindowBlock : Window
{
    private readonly IBlockService blockService;

    public WindowBlock(IServiceProvider services)
    {
        InitializeComponent();
        this.blockService = services.GetRequiredService<IBlockService>();
    }


    private async void btnTasdiqlash_Click(object sender, RoutedEventArgs e)
    {
        string password = tbUsername.Text + DateTimeOffset.Now.Date.AddDays(10).ToString().Substring(0, 10);

        var term = tbUsername.Text.Substring(tbUsername.Text.Length - 2);

        // necha oy ishlanini aniqlab olamiz
        int months = int.Parse(term);
        
        // Hash the password
        //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        try
        {
            // Verify the password
            bool isVerified = BCrypt.Net.BCrypt.Verify(password, tbParol.Text);
            if (isVerified)
            {
                var result = await blockService.UpdateAsync(months);

                if (result.StatusCode.Equals(200))
                {
                    MessageBox.Show($"{result.Data.EndDate.ToString().Substring(0, 11)} gacha to'liq ishlaydi.");
                    this.Close();
                }
            }
        }
        catch
        {
            MessageBox.Show("Iltimos yuqoridagi manzillarga murojaat qiling. O'zgalar mehnatini qadrlang. ");
        }
    }
}
