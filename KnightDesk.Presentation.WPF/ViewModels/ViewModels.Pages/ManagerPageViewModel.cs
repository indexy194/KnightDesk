using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using KnightDesk.Presentation.WPF.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class ManagerPageViewModel : INotifyPropertyChanged
    {
        private readonly IAccountServices _accountService;
        private readonly ITcpServices _tcp;

        private ObservableCollection<Account> _allAccounts;
        private ObservableCollection<Account> _filteredAccounts;
        
        private Account _selectedAccount;
        private string _searchText;
        private bool _isLoading;
        private GameConfig _gameConfig;
        
        private readonly Dictionary<int, Account> _activeAccounts = new Dictionary<int, Account>();
        private readonly object _accountLock = new object();
        private readonly int _tcpPort = TcpConstants.DEFAULT_TCP_PORT;

        public ManagerPageViewModel()
        {
            _accountService = new AccountServices();
            _tcp = new TcpServices();
            _allAccounts = new ObservableCollection<Account>();
            _filteredAccounts = new ObservableCollection<Account>();
            _gameConfig = new GameConfig();

            // Initialize commands
            SearchCommand = new RelayCommand(ExecuteSearch);
            SelectAccountCommand = new RelayCommand<Account>(ExecuteSelectAccount);
            StartStopGameCommand = new RelayCommand<Account>(ExecuteStartStopGame);
            CheckGameConnectionCommand = new RelayCommand<Account>(ExecuteCheckGameConnection);

            // Auto settings commands
            ToggleAutoStateCommand = new RelayCommand(ExecuteToggleAutoState);
            ToggleAutoEventCommand = new RelayCommand(ExecuteToggleAutoEvent);
            ToggleAutoEquipCommand = new RelayCommand(ExecuteToggleAutoEquip);
            SendNameFocusCommand = new RelayCommand(ExecuteSendNameFocus);
            SendMountCommand = new RelayCommand(ExecuteSendMount); // Placeholder for mount command

            // Subscribe to TCP events
            _tcp.CommandReceived += OnTcpCommandReceived;
            _tcp.ClientConnected += OnTcpClientConnected;
            _tcp.ClientDisconnected += OnTcpClientDisconnected;

            // Load game config
            LoadGameConfig();

            // Load favorite accounts on initialization
            LoadFavoriteAccounts();
        }

        #region Properties

        // Static lists for ComboBox binding - expose through ViewModel
        public List<string> EventNames { get { return AutoSettings.EventNames; } }
        public List<string> EquipTypeNames { get { return AutoSettings.EquipTypeNames; } }
        public List<MountTypeItem> MountTypeItems
        {
            get
            {
                var items = new List<MountTypeItem>();
                foreach (var mountType in AutoSettings.MountTypes)
                {
                    items.Add(new MountTypeItem(mountType));
                }
                return items;
            }
        }

        public ObservableCollection<Account> FilteredAccounts
        {
            get { return _filteredAccounts; }
            set
            {
                _filteredAccounts = value;
                OnPropertyChanged("FilteredAccounts");
            }
        }

        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                OnPropertyChanged("SelectedAccount");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                // Auto search when text changes
                ExecuteSearch();
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

        public GameConfig GameConfig
        {
            get { return _gameConfig; }
            set
            {
                _gameConfig = value;
                OnPropertyChanged("GameConfig");
            }
        }

        public bool HasRunningGames
        {
            get
            {
                if (_filteredAccounts == null) return false;
                foreach (var account in _filteredAccounts)
                {
                    if (account.IsGameRunning) return true;
                }
                return false;
            }
        }

        public bool CanUseAutoControls
        {
            get { return _selectedAccount != null && _selectedAccount.CanUseGameControls; }
        }

        public bool IsConnectionStatusVisible
        {
            get { return _selectedAccount != null && _selectedAccount.IsConnectedToGame; }
        }

        public string ConnectionStatusText
        {
            get
            {
                if (_selectedAccount != null)
                    return string.Format("Auto: {0}", _selectedAccount.GameStatus);
                return "Auto: Offline";
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; }
        public ICommand SelectAccountCommand { get; private set; }
        public ICommand StartStopGameCommand { get; private set; }
        public ICommand CheckGameConnectionCommand { get; private set; }

        // Auto settings commands
        public ICommand ToggleAutoStateCommand { get; private set; }
        public ICommand ToggleAutoEventCommand { get; private set; }
        public ICommand ToggleAutoEquipCommand { get; private set; }
        public ICommand SendNameFocusCommand { get; private set; }
        public ICommand SendMountCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteSearch()
        {
            if (string.IsNullOrEmpty(_searchText))
            {
                FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
            }
            else
            {
                var searchLower = _searchText.ToLower();
                var filteredList = new List<Account>();
                foreach (var account in _allAccounts)
                {
                    bool matches = false;
                    if (!string.IsNullOrEmpty(account.Username) && account.Username.ToLower().Contains(searchLower))
                        matches = true;
                    if (!string.IsNullOrEmpty(account.CharacterName) && account.CharacterName.ToLower().Contains(searchLower))
                        matches = true;

                    if (matches)
                        filteredList.Add(account);
                }
                FilteredAccounts = new ObservableCollection<Account>(filteredList);
            }
        }


        private void ExecuteSelectAccount(object parameter)
        {
            if (parameter is Account account)
            {
                SelectedAccount = account;
                OnPropertyChanged("CanUseAutoControls");
                OnPropertyChanged("IsConnectionStatusVisible");
                OnPropertyChanged("ConnectionStatusText");
            }
        }

        private void ExecuteStartStopGame(Account account)
        {
            if (account == null) return;

            if (account.IsGameRunning)
            {
                // Stop game
                StopGame(account);
            }
            else
            {
                Console.WriteLine($"Starting game for account {account.Id}");
                // Start game
                StartGame(account);
            }
        }

        private void ExecuteCheckGameConnection(Account account)
        {
            if (account == null || !account.IsGameRunning) return;

            CheckGameConnection(account);
            OnPropertyChanged(nameof(CanUseAutoControls));
            OnPropertyChanged(nameof(IsConnectionStatusVisible));
            OnPropertyChanged(nameof(ConnectionStatusText));
        }

        private void ExecuteToggleAutoState()
        {
            if (_selectedAccount?.AutoSettings != null && _selectedAccount.IsGameRunning && _selectedAccount.IsConnectedToGame)
            {
                _selectedAccount.AutoSettings.AutoState = !_selectedAccount.AutoSettings.AutoState;
                SendAutoCommand(GameCommandConstants.AUTO_STATE_COMMAND, _selectedAccount.AutoSettings.AutoState.ToString());
            }
        }

        private void ExecuteToggleAutoEvent()
        {
            if (_selectedAccount?.AutoSettings != null && _selectedAccount.IsGameRunning && _selectedAccount.IsConnectedToGame)
            {
                _selectedAccount.AutoSettings.AutoEvent = !_selectedAccount.AutoSettings.AutoEvent;
                SendAutoCommandWithName(GameCommandConstants.AUTO_EVENT_COMMAND, 
                    _selectedAccount.AutoSettings.AutoEvent.ToString(), 
                    _selectedAccount.AutoSettings.EventName);
            }
        }

        private void ExecuteToggleAutoEquip()
        {
            if (_selectedAccount?.AutoSettings != null && _selectedAccount.IsGameRunning && _selectedAccount.IsConnectedToGame)
            {
                _selectedAccount.AutoSettings.AutoEquip = !_selectedAccount.AutoSettings.AutoEquip;
                SendAutoCommandWithName(GameCommandConstants.AUTO_EQUIP_COMMAND, 
                    _selectedAccount.AutoSettings.AutoEquip.ToString(), 
                    _selectedAccount.AutoSettings.EquipTypeName);
            }
        }

        private void SendAutoCommand(string command, string status, string info = null)
        {
            if (_selectedAccount != null && _selectedAccount.CanUseGameControls)
            {
                // Check if client is still connected before sending command
                if (!_tcp.IsClientConnected(_selectedAccount.Id))
                {
                    Console.WriteLine($"Cannot send command to account {_selectedAccount.Id}: client not connected");
                    return;
                }

                var fullCommand = ConvertMsg(command, status, info);
                Console.WriteLine(string.Format("ID:{0}, cmd:{1}", _selectedAccount.Id, fullCommand));
                _tcp.SendCommandToGameAsync(_selectedAccount.Id, fullCommand, (success) =>
                {
                    if (!success)
                    {
                        Console.WriteLine($"Failed to send command to account {_selectedAccount.Id}");
                        // Update connection status if command failed
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _selectedAccount.IsConnectedToGame = false;
                            _selectedAccount.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                            OnPropertyChanged(nameof(CanUseAutoControls));
                            OnPropertyChanged(nameof(IsConnectionStatusVisible));
                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    }
                });
            }
        }

        private void SendAutoCommandWithName(string command, string value, string name)
        {
            if (_selectedAccount != null && _selectedAccount.CanUseGameControls)
            {
                // Check if client is still connected before sending command
                if (!_tcp.IsClientConnected(_selectedAccount.Id))
                {
                    Console.WriteLine($"Cannot send command to account {_selectedAccount.Id}: client not connected");
                    return;
                }

                var fullCommand = ConvertMsg(command, value, name ?? string.Empty);
                Console.WriteLine(string.Format("ID:{0}, cmd:{1}", _selectedAccount.Id, fullCommand));
                _tcp.SendCommandToGameAsync(_selectedAccount.Id, fullCommand, (success) =>
                {
                    if (!success)
                    {
                        Console.WriteLine($"Failed to send command to account {_selectedAccount.Id}");
                        // Update connection status if command failed
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _selectedAccount.IsConnectedToGame = false;
                            _selectedAccount.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                            OnPropertyChanged(nameof(CanUseAutoControls));
                            OnPropertyChanged(nameof(IsConnectionStatusVisible));
                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    }
                });
            }
        }

        private void ExecuteSendNameFocus()
        {
            if (_selectedAccount?.AutoSettings != null && _selectedAccount.IsGameRunning && _selectedAccount.IsConnectedToGame)
            {
                var nameFocus = _selectedAccount.AutoSettings.NameFocus ?? string.Empty;
                
                // Validate that NameFocus is not empty
                if (string.IsNullOrWhiteSpace(nameFocus))
                {
                    Console.WriteLine($"Cannot send NameFocus command: NameFocus is empty for account {_selectedAccount.Id}");
                    return;
                }

                var fullCommand = ConvertMsg(GameCommandConstants.NAME_FOCUS_COMMAND, nameFocus);
                Console.WriteLine(string.Format("ID:{0}, Sending NameFocus: {1}", _selectedAccount.Id, fullCommand));
                _tcp.SendCommandToGameAsync(_selectedAccount.Id, fullCommand, (success) =>
                {
                    if (!success)
                    {
                        Console.WriteLine($"Failed to send NameFocus command to account {_selectedAccount.Id}");
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _selectedAccount.IsConnectedToGame = false;
                            _selectedAccount.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                            OnPropertyChanged(nameof(CanUseAutoControls));
                            OnPropertyChanged(nameof(IsConnectionStatusVisible));
                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    }
                    else
                    {
                        Console.WriteLine($"NameFocus command sent successfully to account {_selectedAccount.Id}: {nameFocus}");
                    }
                });
            }
        }

         private void ExecuteSendMount()
         {
             // MOUNT|MountTypeId|MountType2Id
             if (_selectedAccount?.AutoSettings != null && _selectedAccount.IsGameRunning && _selectedAccount.IsConnectedToGame)
             {
                 var mountTypeId = ((int)_selectedAccount.AutoSettings.MountType).ToString();
                 var mountType2Id = ((int)_selectedAccount.AutoSettings.MountType2).ToString();
                 var fullCommand = ConvertMsg(GameCommandConstants.MOUNT_COMMAND, mountTypeId, mountType2Id);
                 Console.WriteLine(string.Format("ID:{0}, Sending Mount: {1}", _selectedAccount.Id, fullCommand));
                 _tcp.SendCommandToGameAsync(_selectedAccount.Id, fullCommand, (success) =>
                 {
                     if (!success)
                     {
                         Console.WriteLine($"Failed to send Mount command to account {_selectedAccount.Id}");
                         Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                         {
                             _selectedAccount.IsConnectedToGame = false;
                             _selectedAccount.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                             OnPropertyChanged(nameof(CanUseAutoControls));
                             OnPropertyChanged(nameof(IsConnectionStatusVisible));
                             OnPropertyChanged(nameof(ConnectionStatusText));
                         }));
                     }
                     else
                     {
                         Console.WriteLine($"Mount command sent successfully to account {_selectedAccount.Id}: MountType={mountTypeId}, MountType2={mountType2Id}");
                     }
                 });
             }
         }


        #endregion

        #region TCP Event Handlers

        private void OnTcpCommandReceived(int accountId, string command)
        {
            Console.WriteLine($"Received command from account {accountId}: {command}");
            
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (command.StartsWith(GameCommandConstants.CHARACTER_NAME_PREFIX))
                {
                    string characterName = command.Substring(GameCommandConstants.CHARACTER_NAME_PREFIX.Length);
                    HandleCharacterNameReceived(accountId, characterName);
                }
                else if (command == GameCommandConstants.CLIENT_SHUTDOWN_COMMAND)
                {
                    Console.WriteLine($"Client {accountId} requested shutdown");
                    HandleClientShutdown(accountId);
                }
                else if (command == GameCommandConstants.PONG_RESPONSE)
                {
                    // Handle ping response
                    UpdateAccountConnectionStatus(accountId, true);
                }
            }));
        }

        private void OnTcpClientConnected(int accountId)
        {
            Console.WriteLine($"Game client connected for account {accountId}");
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateAccountConnectionStatus(accountId, true);
                OnPropertyChanged(nameof(CanUseAutoControls));
                OnPropertyChanged(nameof(IsConnectionStatusVisible));
                OnPropertyChanged(nameof(ConnectionStatusText));
            }));
        }

        private void OnTcpClientDisconnected(int accountId)
        {
            Console.WriteLine($"Game client disconnected for account {accountId}");
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateAccountConnectionStatus(accountId, false);
                OnPropertyChanged(nameof(CanUseAutoControls));
                OnPropertyChanged(nameof(IsConnectionStatusVisible));
                OnPropertyChanged(nameof(ConnectionStatusText));
            }));
        }

        #endregion

        #region Game Process Management

        private void StartGame(Account account)
        {
            if (string.IsNullOrEmpty(_gameConfig.GamePath) || !File.Exists(_gameConfig.GamePath))
            {
                MessageBox.Show("Game path is not configured or invalid. Please configure game path in settings.",
                              "Game Path Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    // Start TCP server if not already running
                    if (!_tcp.IsServerRunning)
                    {
                        StartTcpServer();
                        // Wait a moment for TCP server to start
                        Thread.Sleep(1000);
                    }

                    // config port
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = _gameConfig.GamePath,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        Arguments = string.Format("{0} {1} {2} {3}",
                            GameConfigConstants.ACCOUNT_ARGUMENT, account.Id,
                            GameConfigConstants.TCP_PORT_ARGUMENT, _tcpPort)
                    };

                    var process = Process.Start(processStartInfo);
                    if (process != null)
                    {
                        account.GameProcess = process;
                        account.IsGameRunning = true;
                        account.GameStatus = GameCommandConstants.STATUS_STARTING;

                        // Register account as active
                        RegisterActiveAccount(account);

                        // Monitor process in background
                        ThreadPool.QueueUserWorkItem(__ => MonitorGameProcess(account));

                        // Send login command after a delay
                        ThreadPool.QueueUserWorkItem(___ =>
                        {
                            Thread.Sleep(TcpConstants.GAME_INIT_DELAY_MS); // Wait for game to initialize
                            SendLoginCommand(account);
                        });

                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    account.GameStatus = string.Format("Error: {0}", ex.Message);
                    result = false;
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (result)
                    {
                        account.GameStatus = GameCommandConstants.STATUS_LOGGING_IN;
                    }
                    else
                    {
                        account.GameStatus = GameCommandConstants.STATUS_START_FAILED;
                    }

                    OnPropertyChanged(nameof(HasRunningGames));
                    OnPropertyChanged(nameof(CanUseAutoControls));
                    OnPropertyChanged(nameof(IsConnectionStatusVisible));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                }));
            });
        }

        private void StopGame(Account account)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // First, update account status to prevent new commands
                    account.IsGameRunning = false;
                    account.IsConnectedToGame = false;
                    account.GameStatus = GameCommandConstants.STATUS_OFFLINE;

                    // Unregister account from active accounts before stopping TCP
                    UnregisterActiveAccount(account.Id);

                    // Send shutdown command to game client if still connected
                    if (_tcp.IsClientConnected(account.Id))
                    {
                        try
                        {
                            _tcp.SendCommandToGame(account.Id, GameCommandConstants.CLIENT_SHUTDOWN_COMMAND);
                            Thread.Sleep(500); // Give time for graceful shutdown
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending shutdown command: {ex.Message}");
                        }
                    }

                    // Now stop the game process
                    if (account.GameProcess != null && !account.GameProcess.HasExited)
                    {
                        // Wait a moment for graceful shutdown
                        Thread.Sleep(TcpConstants.SHUTDOWN_TIMEOUT_MS);

                        // Force kill if still running
                        if (!account.GameProcess.HasExited)
                        {
                            account.GameProcess.Kill();
                        }

                        account.GameProcess.Dispose();
                        account.GameProcess = null;
                    }

                    // Check if TCP server should be stopped (after all cleanup)
                    CheckAndStopTcpServerIfNeeded();

                }
                catch (Exception ex)
                {
                    account.GameStatus = string.Format("Error stopping: {0}", ex.Message);
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OnPropertyChanged(nameof(HasRunningGames));
                    OnPropertyChanged(nameof(CanUseAutoControls));
                    OnPropertyChanged(nameof(IsConnectionStatusVisible));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                }));
            });
        }

        private void CheckGameConnection(Account account)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    if (!account.IsGameRunning || account.GameProcess == null || account.GameProcess.HasExited)
                    {
                        account.IsConnectedToGame = false;
                        result = false;
                    }
                    else
                    {
                        // Check TCP connection
                        bool connected = _tcp.IsClientConnected(account.Id);
                        account.IsConnectedToGame = connected;

                        if (connected)
                        {
                            account.GameStatus = GameCommandConstants.STATUS_IN_GAME;
                        }
                        else
                        {
                            account.GameStatus = GameCommandConstants.STATUS_NO_CONNECTION;
                        }

                        result = connected;
                    }
                }
                catch (Exception)
                {
                    account.IsConnectedToGame = false;
                    account.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                    result = false;
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OnPropertyChanged(nameof(CanUseAutoControls));
                    OnPropertyChanged(nameof(IsConnectionStatusVisible));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                }));
            });
        }

        private void SendLoginCommand(Account account)
        {
            if (account == null) return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // Wait a moment for game to fully start
                    Thread.Sleep(TcpConstants.LOGIN_DELAY_MS);

                    string loginCommand = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}",
                        GameCommandConstants.LOGIN_COMMAND,
                        GameCommandConstants.COMMAND_SEPARATOR,
                        account.Username,
                        account.Password,
                        account.IndexCharacter,
                        account.IndexServer);

                    _tcp.SendCommandToGame(account.Id, loginCommand, (response) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (!string.IsNullOrEmpty(response) && response.StartsWith(GameCommandConstants.CHARACTER_NAME_PREFIX))
                            {
                                string characterName = response.Substring(GameCommandConstants.CHARACTER_NAME_PREFIX.Length);
                                account.CharacterName = characterName;
                                account.GameStatus = string.Format("Logged in as {0}", characterName);

                                // Update character name via API
                                UpdateCharacterNameViaAPI(account, characterName);
                            }
                            else
                            {
                                account.GameStatus = GameCommandConstants.STATUS_LOGIN_FAILED;
                            }

                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    });
                }
                catch
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        account.GameStatus = GameCommandConstants.STATUS_LOGIN_ERROR;
                        OnPropertyChanged(nameof(ConnectionStatusText));
                    }));
                }
            });
        }

        private void MonitorGameProcess(Account account)
        {
            try
            {
                while (account.GameProcess != null && !account.GameProcess.HasExited)
                {
                    Thread.Sleep(TcpConstants.MONITOR_INTERVAL_MS); // Check every 5 seconds

                    // Simple TCP connection check
                    try
                    {
                        bool connected = _tcp.IsClientConnected(account.Id);
                        account.IsConnectedToGame = connected;

                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (connected)
                            {
                                account.GameStatus = GameCommandConstants.STATUS_IN_GAME;
                            }
                            else
                            {
                                account.GameStatus = GameCommandConstants.STATUS_NO_CONNECTION;
                            }
                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    }
                    catch (Exception)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            account.IsConnectedToGame = false;
                            account.GameStatus = GameCommandConstants.STATUS_CONNECTION_ERROR;
                            OnPropertyChanged(nameof(ConnectionStatusText));
                        }));
                    }
                }

                // Process has exited
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (account.GameProcess != null)
                    {
                        account.GameProcess.Dispose();
                        account.GameProcess = null;
                    }

                    account.IsGameRunning = false;
                    account.IsConnectedToGame = false;
                        account.GameStatus = GameCommandConstants.STATUS_OFFLINE;

                    // Unregister account from active accounts
                    UnregisterActiveAccount(account.Id);

                    CheckAndStopTcpServerIfNeeded();

                    OnPropertyChanged(nameof(HasRunningGames));
                    OnPropertyChanged(nameof(CanUseAutoControls));
                    OnPropertyChanged(nameof(IsConnectionStatusVisible));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                }));
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Handle monitoring errors silently
                    account.IsGameRunning = false;
                    account.IsConnectedToGame = false;
                    account.GameStatus = GameCommandConstants.STATUS_MONITORING_ERROR;

                    // Unregister account from active accounts
                    UnregisterActiveAccount(account.Id);

                    CheckAndStopTcpServerIfNeeded();

                    OnPropertyChanged(nameof(HasRunningGames));
                    OnPropertyChanged(nameof(CanUseAutoControls));
                    OnPropertyChanged(nameof(IsConnectionStatusVisible));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                }));
            }
        }

        private void HandleClientShutdown(int accountId)
        {
            Console.WriteLine($"Handling client shutdown for account {accountId}");

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (_accountLock)
                {
                    if (_activeAccounts.ContainsKey(accountId))
                    {
                        var account = _activeAccounts[accountId];
                        StopGame(account);
                    }
                    else
                    {
                        Console.WriteLine($"Account {accountId} not found in active accounts");
                    }
                }
            }));
        }

        private void HandleCharacterNameReceived(int accountId, string characterName)
        {
            lock (_accountLock)
            {
                if (_activeAccounts.ContainsKey(accountId))
                {
                    var account = _activeAccounts[accountId];
                    UpdateCharacterNameViaAPI(account, characterName);
                }
            }
        }

        private void UpdateCharacterNameViaAPI(Account account, string characterName)
        {
            if (account != null && !string.IsNullOrEmpty(characterName))
            {
                var updateDto = new UpdateCharacterDTO
                {
                    Id = account.Id,
                    CharacterName = characterName
                };
                _accountService.UpdateCharacterNameByUserId(updateDto, new Action<GeneralResponseDTO<bool>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == (int)RESPONSE_CODE.OK && result.Data)
                        {
                            account.CharacterName = characterName;
                            // Notify UI of change
                            OnPropertyChanged("FilteredAccounts");
                            OnPropertyChanged("SelectedAccount");
                        }
                    }));
                }));
            }
        }

        private void UpdateAccountConnectionStatus(int accountId, bool connected)
        {
            lock (_accountLock)
            {
                if (_activeAccounts.ContainsKey(accountId))
                {
                    var account = _activeAccounts[accountId];
                    account.IsConnectedToGame = connected;
                    if (connected)
                    {
                        account.GameStatus = GameCommandConstants.STATUS_IN_GAME;
                    }
                    else
                    {
                        account.GameStatus = GameCommandConstants.STATUS_DISCONNECTED;
                    }
                }
            }
        }

        private void RegisterActiveAccount(Account account)
        {
            lock (_accountLock)
            {
                _activeAccounts[account.Id] = account;
            }
        }

        private void UnregisterActiveAccount(int accountId)
        {
            lock (_accountLock)
            {
                _activeAccounts.Remove(accountId);
            }
        }

        private void CheckAndStopTcpServerIfNeeded()
        {
            // Check if there are any other running games
            bool hasOtherRunningGames = false;
            lock (_accountLock)
            {
                foreach (var activeAccount in _activeAccounts.Values)
                {
                    if (activeAccount.IsGameRunning)
                    {
                        hasOtherRunningGames = true;
                        break;
                    }
                }
            }

            // Stop TCP server if no games are running
            if (!hasOtherRunningGames && _tcp.IsServerRunning)
            {
                _tcp.StopTcpServer();
                Console.WriteLine("ManagerPageViewModel: TCP server stopped - no games running");
            }
        }

        private void StartTcpServer()
        {
            // Check if server is already running
            if (_tcp.IsServerRunning)
            {
                Console.WriteLine("ManagerPageViewModel: TCP server is already running");
                return;
            }

            _tcp.StartTcpServer(_tcpPort, (success) =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (success)
                    {
                        Console.WriteLine($"ManagerPageViewModel: TCP server started successfully");
                    }
                    else
                    {
                        Console.WriteLine("ManagerPageViewModel: Failed to start TCP server - port may be in use");
                        MessageBox.Show("Failed to start TCP server. Port may be in use by another application.", 
                                      "TCP Server Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }));
            });
        }

        #endregion

        #region Private Methods
        private string ConvertMsg(params string[] parts)
        {
            return string.Join(GameCommandConstants.COMMAND_SEPARATOR, parts);
        }
        private void LoadGameConfig()
        {
            // Load game path from settings
            var savedGamePath = Properties.Settings.Default.GamePath;
            if (!string.IsNullOrEmpty(savedGamePath))
            {
                _gameConfig.GamePath = savedGamePath;
            }
        }

        private void LoadFavoriteAccounts()
        {
            IsLoading = true;
            int userId = Properties.Settings.Default.UserId;
            if (userId <= 0)
            {
                IsLoading = false;
                return;
            }
            // Use the correct API endpoint for getting favorite accounts by user ID
            _accountService.GetFavoriteAccountsByUserIdAsync(userId, new Action<GeneralResponseDTO<IEnumerable<AccountDTO>>>(result =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (result.Code == (int)RESPONSE_CODE.OK && result.Data != null)
                    {
                        foreach (var entry in result.Data)
                        {
                            Account account = new Account
                            {
                                Id = entry.Id,
                                Username = entry.Username,
                                CharacterName = entry.CharacterName,
                                Password = entry.Password,
                                IndexCharacter = entry.IndexCharacter,
                                IsFavorite = entry.IsFavorite,
                                ServerInfoId = entry.ServerInfoId,
                                IndexServer = entry.ServerInfo.IndexServer,
                                ServerName = entry.ServerInfo.Name,
                            };
                            _allAccounts.Add(account);
                        }
                        FilteredAccounts = _allAccounts;

                        // Set first account as selected if available
                        if (_allAccounts.Count > 0)
                        {
                            SelectedAccount = _allAccounts[0];
                        }
                    }
                    IsLoading = false;
                }));
            }));
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            try
            {
                // Stop TCP server
                if (_tcp.IsServerRunning)
                {
                    _tcp.StopTcpServer();
                }

                // Unsubscribe from events
                _tcp.CommandReceived -= OnTcpCommandReceived;
                _tcp.ClientConnected -= OnTcpClientConnected;
                _tcp.ClientDisconnected -= OnTcpClientDisconnected;

                // Stop all running games
                lock (_accountLock)
                {
                    foreach (var account in _activeAccounts.Values)
                    {
                        if (account.IsGameRunning)
                        {
                            StopGame(account);
                        }
                    }
                }

                Console.WriteLine("ManagerPageViewModel: Cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ManagerPageViewModel: Error during cleanup: {ex.Message}");
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
