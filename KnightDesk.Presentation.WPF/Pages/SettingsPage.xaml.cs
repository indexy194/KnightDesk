using KnightDesk.Presentation.WPF.ViewModels.Pages;
using System.Windows.Controls;

namespace KnightDesk.Presentation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsPageViewModel();
        }
    }
}
