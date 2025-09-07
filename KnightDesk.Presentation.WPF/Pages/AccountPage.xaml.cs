using KnightDesk.Presentation.WPF.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;

namespace KnightDesk.Presentation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for AccountPage.xaml
    /// </summary>
    public partial class AccountPage : UserControl
    {
        public AccountPage()
        {
            InitializeComponent();
            DataContext = new AccountPageViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AccountPageViewModel viewModel)
            {
                viewModel.CurrentPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
