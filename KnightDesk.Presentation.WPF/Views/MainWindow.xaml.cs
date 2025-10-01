using KnightDesk.Presentation.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        
        protected override void OnClosed(System.EventArgs e)
        {
            // Cleanup resources when window is closed
            _viewModel?.Cleanup();
            base.OnClosed(e);
        }
    }
}
