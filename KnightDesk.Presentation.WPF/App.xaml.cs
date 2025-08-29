using KnightDesk.Presentation.WPF.Services;
using KnightDesk.Presentation.WPF.ViewModels;
using System;
using System.Configuration;
using System.Windows;

namespace KnightDesk.Presentation.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IAccountApiService _accountApiService;
        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            //try
            //{
            //    // Get API base URL from app.config or use default
            //    string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7001";

            //    // Initialize services
            //    _accountApiService = new AccountApiService(apiBaseUrl);

            //    // Initialize ViewModels
            //    var accountManagementViewModel = new AccountManagementViewModel(_accountApiService);
            //    var mainWindowViewModel = new MainWindowViewModel(accountManagementViewModel);

            //    // Create and show main window
            //    _mainWindow = new MainWindow();
            //    _mainWindow.DataContext = mainWindowViewModel;
            //    _mainWindow.Show();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Application startup failed: {ex.Message}", 
            //        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Shutdown(1);
            //}

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //// Cleanup resources
            //if (_accountApiService is AccountApiService service)
            //{
            //    service.Dispose();
            //}

            base.OnExit(e);
        }
    }
}
