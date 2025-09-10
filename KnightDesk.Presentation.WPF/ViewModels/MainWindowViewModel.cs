using KnightDesk.Presentation.WPF.Pages;
using KnightDesk.Presentation.WPF.ViewModels.Pages;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private UserControl _currentPage;
        private UserControl _managerPage;
        private string _username;
        private string _userGreeting;
        private string _activeMenu = "Manager";

        public MainWindowViewModel()
        {
            // Initialize pages
            _managerPage = new ManagerPage();

            // Load username and set greeting
            LoadUserInfo();

            // Initialize commands - .NET 3.5 compatible
            NavigateToManagerCommand = new RelayCommand(new Action(() => NavigateToPage("Manager")));
            NavigateToAccountsCommand = new RelayCommand(new Action(() => NavigateToPage("Accounts")));
            NavigateToConfigCommand = new RelayCommand(new Action(() => NavigateToPage("Config")));
            NavigateToServerConfigCommand = new RelayCommand(new Action(() => NavigateToPage("ServerConfig")));
            LogoutCommand = new RelayCommand(new Action(ExecuteLogout));
            CloseAppCommand = new RelayCommand(new Action(() => System.Windows.Application.Current.Shutdown()));

            // Set default page
            NavigateToPage("Manager");
        }

        #region Properties

        public UserControl CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("DisplayedPage");
            }
        }

        public string ActiveMenu
        {
            get { return _activeMenu; }
            set
            {
                _activeMenu = value;
                OnPropertyChanged("ActiveMenu");
                OnPropertyChanged("IsManagerActive");
                OnPropertyChanged("IsAccountsActive");
                OnPropertyChanged("IsConfigActive");
                OnPropertyChanged("IsServerConfigActive");
            }
        }

        public bool IsManagerActive
        {
            get { return ActiveMenu == "Manager"; }
        }

        public bool IsAccountsActive
        {
            get { return ActiveMenu == "Accounts"; }
        }

        public bool IsConfigActive
        {
            get { return ActiveMenu == "Config"; }
        }

        public bool IsServerConfigActive
        {
            get { return ActiveMenu == "ServerConfig"; }
        }

        public UserControl ManagerPage
        {
            get { return _managerPage; }
        }

        public UserControl DisplayedPage
        {
            get
            {
                return _currentPage ?? _managerPage;
            }
        }
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
                UpdateUserGreeting();
            }
        }

        public string UserGreeting
        {
            get { return _userGreeting; }
            set
            {
                _userGreeting = value;
                OnPropertyChanged("UserGreeting");
            }
        }

        #endregion

        #region Commands

        public ICommand NavigateToManagerCommand { get; private set; }
        public ICommand NavigateToAccountsCommand { get; private set; }
        public ICommand NavigateToConfigCommand { get; private set; }
        public ICommand NavigateToServerConfigCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public ICommand CloseAppCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void NavigateToPage(string pageName)
        {
            ActiveMenu = pageName;

            switch (pageName)
            {
                case "Manager":
                    CurrentPage = new ManagerPage(); // This will show the original content
                    break;
                case "Accounts":
                    CurrentPage = new AccountPage();
                    break;
                case "Config":
                    CurrentPage = new SettingsPage();
                    break;
                case "ServerConfig":
                    CurrentPage = new ServerConfigPage();
                    break;
                default:
                    CurrentPage = null;
                    break;
            }
        }
        private void ExecuteLogout()
        {
            try
            {
                // Clear saved user data
                Properties.Settings.Default.UserId = 0;
                Properties.Settings.Default.Username = string.Empty;
                Properties.Settings.Default.RememberMe = false;
                Properties.Settings.Default.Save();

                // Show login window
                var loginWindow = new Views.LoginWindow();
                loginWindow.Show();

                // Find and close the MainWindow
                foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
                {
                    if (window is Views.MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private Methods

        private void LoadUserInfo()
        {
            // Load username from settings
            var savedUsername = Properties.Settings.Default.Username;
            if (!string.IsNullOrEmpty(savedUsername))
            {
                Username = savedUsername;
            }
        }

        private void UpdateUserGreeting()
        {
            if (!string.IsNullOrEmpty(_username))
            {
                UserGreeting = string.Format("Hello, {0}", _username);
            }
            else
            {
                UserGreeting = "Hello, Guest";
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}