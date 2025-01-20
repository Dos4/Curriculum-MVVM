using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.ServiceProviderInitialization;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModel;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Foxminded.Curriculum.App;

public partial class App : Application
{
    private ILogger<App>? _logger;
    protected override void OnStartup(StartupEventArgs e)
    {
        var provider = new ServiceCollection().GetServiceProvider();
        var dialogService = provider.GetRequiredService<IDialogService>();
        _logger = provider.GetRequiredService<ILogger<App>>();
        _logger.LogInformation("Application startup initiated.");

        try
        {
            _logger.LogInformation("Resolving MainWindow and MainViewModel...");
            var mainWindow = provider.GetRequiredService<MainWindow>();
            var mainViewModel = provider.GetRequiredService<MainViewModel>();

            _logger.LogInformation("Setting the initial CurrentViewModel to HomeViewModel...");
            mainViewModel.CurrentViewModel = provider.GetRequiredService<HomeViewModel>();

            mainWindow.DataContext = mainViewModel;
            _logger.LogInformation("MainWindow DataContext is set. Showing MainWindow...");
            mainWindow.Show();

            base.OnStartup(e);
            _logger.LogInformation("Application startup completed successfully.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            dialogService.ShowCustomError(TechnicalMessage.ExceptionText);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _logger!.LogInformation("Application is shutting down.");
        base.OnExit(e);
    }
}
