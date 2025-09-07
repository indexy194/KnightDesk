using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ILoginService _loginService;
        private string _username;
        private string _password;
        private bool _isLoading;
        private string _errorMessage;
        private bool _rememberMe;

        public LoginViewModel()
        {
            _loginService = new LoginService();
            
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

            // Check server connection first
            ServerConnectionService.CheckServerAsync(Properties.Settings.Default.BaseUrl, new Action<bool>(isConnected =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (!isConnected)
                    {
                        IsLoading = false;
                        ErrorMessage = "Không thể kết nối tới server. Vui lòng kiểm tra kết nối mạng.";
                        return;
                    }

                    // Server is reachable, proceed with login
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoLogin));
                }));
            }));
        }

        private void DoLogin(object state)
        {
            _loginService.LoginAsync(_username, _password, new Action<GeneralResponseDTO<LoginResponseDTO>>(result =>
            {
                // Switch back to UI thread
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (result.Data.IsSuccess)
                    {
                        // Lưu thông tin user
                        Properties.Settings.Default.Username = result.Data.Username;
                        Properties.Settings.Default.UserId = result.Data.Id;
                        Properties.Settings.Default.RememberMe = _rememberMe;
                        
                        // Lưu mật khẩu mã hóa nếu người dùng chọn ghi nhớ
                        if (_rememberMe)
                        {
                            Properties.Settings.Default.EncryptedPassword = EncryptionService.EncryptPassword(_password);
                        }
                        else
                        {
                            Properties.Settings.Default.EncryptedPassword = string.Empty;
                        }
                        
                        Properties.Settings.Default.Save();

                        // Chuyển đến MainWindow
                        var mainWindow = new Views.MainWindow();
                        mainWindow.Show();
                        
                        // Đóng LoginWindow
                        foreach (System.Windows.Window window in Application.Current.Windows)
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
            }));
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

        private void LoadSavedCredentials()
        {
            try
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Username))
                {
                    Username = Properties.Settings.Default.Username;
                }
                
                RememberMe = Properties.Settings.Default.RememberMe;
                
                // Load encrypted password if remember me is enabled
                if (RememberMe && !string.IsNullOrEmpty(Properties.Settings.Default.EncryptedPassword))
                {
                    Password = EncryptionService.DecryptPassword(Properties.Settings.Default.EncryptedPassword);
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
                // Debug info
                System.Diagnostics.Debug.WriteLine($"TryAutoLogin - RememberMe: {Properties.Settings.Default.RememberMe}");
                System.Diagnostics.Debug.WriteLine($"TryAutoLogin - UserId: {Properties.Settings.Default.UserId}");
                System.Diagnostics.Debug.WriteLine($"TryAutoLogin - Username: {Properties.Settings.Default.Username}");
                System.Diagnostics.Debug.WriteLine($"TryAutoLogin - HasEncryptedPassword: {!string.IsNullOrEmpty(Properties.Settings.Default.EncryptedPassword)}");

                // Only auto login if we have all required saved credentials
                if (Properties.Settings.Default.RememberMe && 
                    Properties.Settings.Default.UserId > 0 &&
                    !string.IsNullOrEmpty(Properties.Settings.Default.Username) &&
                    !string.IsNullOrEmpty(Properties.Settings.Default.EncryptedPassword))
                {
                    System.Diagnostics.Debug.WriteLine("TryAutoLogin - All conditions met, checking server connection...");
                    
                    // Check server connection first before auto login
                    ServerConnectionService.CheckServerAsync(Properties.Settings.Default.BaseUrl, new Action<bool>(isConnected =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            System.Diagnostics.Debug.WriteLine($"TryAutoLogin - Server connected: {isConnected}");
                            System.Diagnostics.Debug.WriteLine($"TryAutoLogin - Username loaded: {!string.IsNullOrEmpty(Username)}");
                            System.Diagnostics.Debug.WriteLine($"TryAutoLogin - Password loaded: {!string.IsNullOrEmpty(Password)}");
                            
                            if (isConnected && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                            {
                                System.Diagnostics.Debug.WriteLine("TryAutoLogin - Starting auto login...");
                                // Auto login with saved credentials
                                ExecuteLogin();
                            }
                        }));
                    }));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TryAutoLogin - Conditions not met for auto login");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TryAutoLogin - Exception: {ex.Message}");
                // If auto login fails, just continue normally
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
