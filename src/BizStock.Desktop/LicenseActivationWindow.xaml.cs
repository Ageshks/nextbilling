using System.Windows;
using BizStock.Application.Common.Licensing;

namespace BizStock.Desktop;

public partial class LicenseActivationWindow : Window
{
    private readonly ILicenseService _licenseService;

    public LicenseActivationWindow(ILicenseService licenseService)
    {
        InitializeComponent();
        _licenseService = licenseService;
    }

    private async void ActivateButton_Click(object sender, RoutedEventArgs e)
    {
        var key = LicenseKeyTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(key))
        {
            StatusText.Text = "Please enter a valid license key.";
            return;
        }

        var result = await _licenseService.ActivateAsync(key, installationId: "desktop-installation");
        if (result.Success)
        {
            StatusText.Text = result.Message;
            StatusText.Foreground = System.Windows.Media.Brushes.DarkGreen;
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
            return;
        }

        StatusText.Text = result.Message;
        StatusText.Foreground = System.Windows.Media.Brushes.Crimson;
    }

    private async void TrialButton_Click(object sender, RoutedEventArgs e)
    {
        var result = await _licenseService.ActivateAsync("BIZSTOCK-ABCD-EFGH-IJKL-MNOP", installationId: "desktop-installation");
        if (result.Success)
        {
            StatusText.Text = "Trial activated successfully.";
            StatusText.Foreground = System.Windows.Media.Brushes.DarkGreen;
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
            return;
        }

        StatusText.Text = result.Message;
        StatusText.Foreground = System.Windows.Media.Brushes.Crimson;
    }
}
