using KnightDesk.Presentation.WPF.ViewModels;
using System.Windows.Controls;

namespace KnightDesk.Presentation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for ServerConfigPage.xaml
    /// </summary>
    public partial class ServerConfigPage : UserControl
    {
        public ServerConfigPage()
        {
            InitializeComponent();
            DataContext = new ServerConfigPageViewModel();
        }
    }
}
