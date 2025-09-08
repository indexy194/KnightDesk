using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ILoginServices _loginService;
        private string _username;
        private string _password;
        private bool _isLoading;
        private string _errorMessage;
        private bool _rememberMe;

        public LoginViewModel()
        {
            _loginService = new LoginServices();

            // Initialize commands - .NET 3.5 compatible
            LoginCommand = new RelayCommand(new Action(ExecuteLogin), new Func<bool>(() => CanExecuteLogin()));
            CloseCommand = new RelayCommand(new Action(ExecuteClose));

            // Load saved credentials if available
            LoadSavedCredentials();

            // Try auto login if credentials are saved
            TryAutoLogin();
        }

        #region Properties

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
                OnPropertyChanged("CanLogin");
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
                OnPropertyChanged("CanLogin");
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
                OnPropertyChanged("HasError");
            }
        }

        public bool HasError
        {
            get { return !string.IsNullOrEmpty(_errorMessage); }
        }

        public bool CanLogin
        {
            get { return !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password) && !_isLoading; }
        }

        public bool RememberMe
        {
            get { return _rememberMe; }
            set
            {
                _rememberMe = value;
                OnPropertyChanged("RememberMe");
            }
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteLogin()
        {
            if (!CanLogin)
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            DoLogin();
        }

        private void DoLogin()
        {
            // Create login request DTO
            var loginRequest = new LoginRequestDTO
            {
                Username = _username,
                Password = _password
            };

            // Use the updated LoginService with DTO (LoginService already uses BackgroundWorker internally)
            _loginService.LoginAsync(loginRequest, result =>
            {
                // LoginService's BackgroundWorker automatically marshals this callback to UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (result.Code == (int)RESPONSE_CODE.OK)
                    {
                        // Lưu thông tin user
                        Properties.Settings.Default.Username = result.Data.Username;
                        Properties.Settings.Default.UserId = result.Data.Id;
                        Properties.Settings.Default.RememberMe = _rememberMe;

                        Properties.Settings.Default.Save();

                        // Chuyển đến MainWindow
                        var mainWindow = new Views.MainWindow();
                        mainWindow.Show();

                        // Đóng LoginWindow
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is Views.LoginWindow)
                            {
                                window.Close();
                                break;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessage = result.Message;
                    }
                    IsLoading = false;
                }));
            });
        }

        private bool CanExecuteLogin()
        {
            return CanLogin;
        }

        private void ExecuteClose()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Private Methods

        private void LoadSavedCredentials() //if user dont want to remember password, just load username
        {
            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Username))
                {
                    Username = Properties.Settings.Default.Username;
                }
            }
            catch (Exception)
            {
                RememberMe = false;
                Password = string.Empty;
            }
        }

        private void TryAutoLogin()
        {
            try
            {
                // First, check network connectivity to server
                var connectionRequest = new ServerConnectionRequestDTO
                {
                    BaseUrl = @"http://localhost:5204",
                    TimeoutMs = 3000
                };

                ServerConnectionServices.CheckServerAsync(connectionRequest, connectionResponse =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (!connectionResponse.IsReachable)
                        {
                            // Server not reachable - stay on login page with error message
                            ErrorMessage = "Không thể kết nối tới server. Vui lòng kiểm tra kết nối mạng.";
                            return;
                        }

                        // Server is reachable - check for saved credentials
                        if (Properties.Settings.Default.RememberMe &&
                            Properties.Settings.Default.UserId > 0 &&
                            !string.IsNullOrEmpty(Properties.Settings.Default.Username))
                        {
                            // Auto login - go directly to main window
                            var mainWindow = new Views.MainWindow();
                            mainWindow.Show();
                            
                            // Close login window
                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window is Views.LoginWindow)
                                {
                                    window.Close();
                                    break;
                                }
                            }
                        }
                       
                    }));
                });
            }
            catch (Exception ex)
            {
                // On any error, stay on login page with error message
                ErrorMessage = string.Format("Lỗi kết nối: {0}", ex.Message);
                RememberMe = false;
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
