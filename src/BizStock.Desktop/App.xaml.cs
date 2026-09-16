using System.Windows;
using BizStock.Application.Common.Licensing;
using Microsoft.Extensions.DependencyInjection;

namespace BizStock.Desktop;

public partial class App : System.Windows.Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddSingleton<ILicenseService, LocalLicenseService>();
        _serviceProvider = services.BuildServiceProvider();

        var licenseService = _serviceProvider.GetRequiredService<ILicenseService>();
        var current = licenseService.GetCurrentLicense();

        if (current is null)
        {
            var activationWindow = new LicenseActivationWindow(licenseService);
            activationWindow.Show();
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

