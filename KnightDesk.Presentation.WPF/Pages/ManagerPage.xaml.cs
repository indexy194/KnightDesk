using System.Windows.Controls;
using KnightDesk.Presentation.WPF.ViewModels.Pages;

namespace KnightDesk.Presentation.WPF.Pages
{
    /// <summary>
    /// Interaction logic for ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : UserControl
    {
        public ManagerPage()
        {
            InitializeComponent();
            DataContext = new ManagerPageViewModel();
        }
    }
}
