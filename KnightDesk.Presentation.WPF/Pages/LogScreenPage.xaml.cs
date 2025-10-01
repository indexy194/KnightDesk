using KnightDesk.Presentation.WPF.ViewModels.ViewModels.Pages;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace KnightDesk.Presentation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for LogScreenPage.xaml
    /// </summary>
    public partial class LogScreenPage : UserControl
    {
        private LogScreenPageViewModel _viewModel;

        public LogScreenPage()
        {
            InitializeComponent();
            _viewModel = new LogScreenPageViewModel();
            DataContext = _viewModel;

            // Subscribe to collection changes for auto-scroll
            if (_viewModel.LogLines != null)
            {
                _viewModel.LogLines.CollectionChanged += OnLogLinesChanged;
            }

            // Handle load/unload events
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;
            
            // Handle visibility changes for tab switching
            IsVisibleChanged += OnVisibilityChanged;
        }

        private void OnLogLinesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        LogScrollViewer.ScrollToEnd();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error auto-scrolling: {ex.Message}");
                    }
                }));
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Refresh log when page is loaded (including when switching back to this tab)
                _viewModel?.RefreshLog();
                Console.WriteLine("LogScreenPage: Page loaded, refreshing log");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogScreenPage: Error during page load: {ex.Message}");
            }
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (IsVisible && _viewModel != null)
                {
                    // Refresh log when page becomes visible (tab switching)
                    _viewModel.RefreshLog();
                    Console.WriteLine("LogScreenPage: Visibility changed to visible, refreshing log");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogScreenPage: Error during visibility change: {ex.Message}");
            }
        }

        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cleanup ViewModel resources
                _viewModel?.Cleanup();

                // Unsubscribe from all events
                if (_viewModel?.LogLines != null)
                {
                    _viewModel.LogLines.CollectionChanged -= OnLogLinesChanged;
                }
                
                Loaded -= OnPageLoaded;
                Unloaded -= OnPageUnloaded;
                IsVisibleChanged -= OnVisibilityChanged;

                Console.WriteLine("LogScreenPage: Cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogScreenPage: Error during cleanup: {ex.Message}");
            }
        }

    }
}
