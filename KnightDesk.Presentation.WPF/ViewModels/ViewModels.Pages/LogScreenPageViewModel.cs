using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace KnightDesk.Presentation.WPF.ViewModels.ViewModels.Pages
{
    public class LogScreenPageViewModel : INotifyPropertyChanged
    {
        private string _logContent;
        private string _logFilePath;
        private DispatcherTimer _refreshTimer;
        private long _lastFileSize = 0;

        public LogScreenPageViewModel()
        {
            // Initialize refresh timer
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += RefreshTimer_Tick;

            // Get log file path from saved settings or default location
            GetLogFilePath();
            
            // Start monitoring
            LoadLogFile();
            StartAutoRefresh();
        }

        #region Properties

        public string LogContent
        {
            get { return _logContent; }
            set
            {
                _logContent = value;
                OnPropertyChanged(nameof(LogContent));
            }
        }

        public string LogFilePath
        {
            get { return _logFilePath; }
            set
            {
                _logFilePath = value;
                OnPropertyChanged(nameof(LogFilePath));
            }
        }

        #endregion

        #region Methods

        private void GetLogFilePath()
        {
            LogFilePath = Properties.Settings.Default.LogPath;
        }

        public ObservableCollection<LogLine> LogLines { get; set; } = new ObservableCollection<LogLine>();

        private void LoadLogFile()
        {
            try
            {
                if (string.IsNullOrEmpty(LogFilePath) || !File.Exists(LogFilePath))
                {
                    LogLines.Clear();
                    LogLines.Add(new LogLine { Text = "Log file not found.", Color = Brushes.Gray });
                    return;
                }

                var fileInfo = new FileInfo(LogFilePath);

                // Check if file has new content
                if (fileInfo.Length == _lastFileSize)
                {
                    return; // No new content
                }

                if (fileInfo.Length < _lastFileSize)
                {
                    // File was truncated, reload everything
                    _lastFileSize = 0;
                    LogLines.Clear();
                }

                string content;
                using (var fileStream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    if (_lastFileSize > 0)
                    {
                        fileStream.Seek(_lastFileSize, SeekOrigin.Begin);
                    }
                    content = reader.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(content))
                {
                    AppendLogLines(content, _lastFileSize == 0);
                }

                _lastFileSize = fileInfo.Length;
                Console.WriteLine($"Loaded log file: {Path.GetFileName(LogFilePath)} ({fileInfo.Length} bytes)");
            }
            catch (Exception ex)
            {
                LogLines.Clear();
                LogLines.Add(new LogLine { Text = $"Error loading log file: {ex.Message}", Color = Brushes.Red });
                Console.WriteLine($"Error loading log file: {ex.Message}");
            }
        }

        private void AppendLogLines(string logContent, bool clearExisting)
        {
            if (clearExisting)
            {
                LogLines.Clear();
            }

            bool toggle = LogLines.Count % 2 != 0;
            foreach (var line in logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var logLine = new LogLine
                {
                    Text = line,
                    Color = toggle ? Brushes.Cyan : Brushes.White
                };

                LogLines.Add(logLine);
                toggle = !toggle;
            }
        }



        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadLogFile();
        }

        private void StartAutoRefresh()
        {
            _refreshTimer.Start();
        }

        public void StopAutoRefresh()
        {
            _refreshTimer?.Stop();
        }

        public void RefreshLog()
        {
            try
            {
                LoadLogFile();
                Console.WriteLine("LogScreenPageViewModel: Manual refresh completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogScreenPageViewModel: Error during manual refresh: {ex.Message}");
            }
        }

        public void ClearLog()
        {
            LogContent = string.Empty;
            _lastFileSize = 0;
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            try
            {
                StopAutoRefresh();
                Console.WriteLine("LogScreenPageViewModel: Cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogScreenPageViewModel: Error during cleanup: {ex.Message}");
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
    public class LogLine
    {
        public string Text { get; set; }
        public Brush Color { get; set; }
    }

}