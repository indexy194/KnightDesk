using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        private string _gamePath = string.Empty;
        private string _dataPath = string.Empty;
        private string _configPath = string.Empty;
        private bool _startWithWindows;
        private bool _minimizeToTray;
        private bool _autoUpdate;

        public SettingsPageViewModel()
        {
            // Initialize commands - .NET 3.5 compatible
            BrowseGamePathCommand = new RelayCommand(new Action(ExecuteBrowseGamePath));
            BrowseDataPathCommand = new RelayCommand(new Action(ExecuteBrowseDataPath));
            BrowseConfigPathCommand = new RelayCommand(new Action(ExecuteBrowseConfigPath));
            SaveSettingsCommand = new RelayCommand(new Action(ExecuteSaveSettings));
            ResetSettingsCommand = new RelayCommand(new Action(ExecuteResetSettings));

            LoadSettings();
        }

        #region Properties



        public string GamePath
        {
            get { return _gamePath; }
            set
            {
                _gamePath = value;
                OnPropertyChanged("GamePath");
            }
        }

        public string DataPath
        {
            get { return _dataPath; }
            set
            {
                _dataPath = value;
                OnPropertyChanged("DataPath");
            }
        }

        public string ConfigPath
        {
            get { return _configPath; }
            set
            {
                _configPath = value;
                OnPropertyChanged("ConfigPath");
            }
        }

        public bool StartWithWindows
        {
            get { return _startWithWindows; }
            set
            {
                _startWithWindows = value;
                OnPropertyChanged("StartWithWindows");
            }
        }

        public bool MinimizeToTray
        {
            get { return _minimizeToTray; }
            set
            {
                _minimizeToTray = value;
                OnPropertyChanged("MinimizeToTray");
            }
        }

        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set
            {
                _autoUpdate = value;
                OnPropertyChanged("AutoUpdate");
            }
        }

        #endregion

        #region Commands

        public ICommand BrowseGamePathCommand { get; private set; }
        public ICommand BrowseDataPathCommand { get; private set; }
        public ICommand BrowseConfigPathCommand { get; private set; }
        public ICommand SaveSettingsCommand { get; private set; }
        public ICommand ResetSettingsCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteBrowseGamePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Select Game Executable File";
            dialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            dialog.InitialDirectory = !string.IsNullOrEmpty(GamePath) ? Path.GetDirectoryName(GamePath) : string.Empty;

            if (dialog.ShowDialog() == true)
            {
                GamePath = dialog.FileName;
            }
        }

        private void ExecuteBrowseDataPath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Data Storage Directory";
            dialog.SelectedPath = DataPath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DataPath = dialog.SelectedPath;
            }
        }

        private void ExecuteBrowseConfigPath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select Configuration Files Directory";
            dialog.SelectedPath = ConfigPath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ConfigPath = dialog.SelectedPath;
            }
        }

        private void ExecuteSaveSettings()
        {
            try
            {
                SaveSettingsToRegistry();
                //ValidateAndCreateDirectories();
                
                System.Windows.MessageBox.Show("Settings saved successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error saving settings: {0}", ex.Message), "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteResetSettings()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to reset all settings to default values?", 
                "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                ResetToDefaults();
                System.Windows.MessageBox.Show("Settings have been reset to default values.", "Reset Complete", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Private Methods

        private void LoadSettings()
        {
            try
            {
                // Load from registry or config file
                //RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\KnightDesk");
                
                GamePath = Properties.Settings.Default.GamePath;
                //DataPath = key.GetValue("DataPath", GetDefaultDataPath()).ToString();
                //ConfigPath = key.GetValue("ConfigPath", GetDefaultConfigPath()).ToString();
                //StartWithWindows = Convert.ToBoolean(key.GetValue("StartWithWindows", false));
                //MinimizeToTray = Convert.ToBoolean(key.GetValue("MinimizeToTray", true));
                //AutoUpdate = Convert.ToBoolean(key.GetValue("AutoUpdate", true));

                //key.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error loading settings: {0}", ex.Message), "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ResetToDefaults();
            }
        }

        private void SaveSettingsToRegistry()
        {
            //RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\KnightDesk");
            
            //key.SetValue("GamePath", GamePath);
            //key.SetValue("DataPath", DataPath);
            //key.SetValue("ConfigPath", ConfigPath);
            //key.SetValue("StartWithWindows", StartWithWindows);
            //key.SetValue("MinimizeToTray", MinimizeToTray);
            //key.SetValue("AutoUpdate", AutoUpdate);

            //key.Close();

            //// Handle Windows startup
            //HandleWindowsStartup();

            //my code
            Properties.Settings.Default.GamePath = GamePath;
            Properties.Settings.Default.Save();
        }

        private void HandleWindowsStartup()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            
            if (StartWithWindows)
            {
                if (startupKey != null)
                {
                    startupKey.SetValue("KnightDesk", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
            }
            else
            {
                if (startupKey != null)
                {
                    try
                    {
                        startupKey.DeleteValue("KnightDesk", false);
                    }
                    catch
                    {
                        // Value doesn't exist, ignore
                    }
                }
            }
            
            if (startupKey != null)
            {
                startupKey.Close();
            }
        }

        //private void ValidateAndCreateDirectories()
        //{
        //    CreateDirectoryIfNotExists(DataPath, "Data Storage");
        //    CreateDirectoryIfNotExists(ConfigPath, "Configuration");
        //}

        private void CreateDirectoryIfNotExists(string path, string description)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(string.Format("Warning: Could not create {0} directory '{1}': {2}", description, path, ex.Message), 
                        "Directory Creation Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ResetToDefaults()
        {
            GamePath = string.Empty;
        }

        private string GetDefaultGamePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Game");
        }

        private string GetDefaultDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Data");
        }

        private string GetDefaultConfigPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Config");
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
