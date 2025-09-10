using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class ManagerPageViewModel : INotifyPropertyChanged
    {
        private readonly IAccountServices _accountService;
        private readonly IGameProcessService _gameProcessService;
        private ObservableCollection<Account> _allAccounts;
        private ObservableCollection<Account> _filteredAccounts;
        private Account _selectedAccount;
        private string _searchText;
        private bool _isLoading;
        private GameConfig _gameConfig;

        public ManagerPageViewModel()
        {
            _accountService = new AccountServices();
            _gameProcessService = new GameProcessService();
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

            // Subscribe to character name received event
            GameProcessService.CharacterNameReceived += OnCharacterNameReceived;

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
                _gameProcessService.StopGame(account, (success) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        OnPropertyChanged(nameof(HasRunningGames));
                        OnPropertyChanged(nameof(CanUseAutoControls));
                        OnPropertyChanged(nameof(IsConnectionStatusVisible));
                        OnPropertyChanged(nameof(ConnectionStatusText));
                    }));
                });
            }
            else
            {
                // Start game
                if (!_gameConfig.IsValidGamePath)
                {
                    MessageBox.Show("Game path is not configured or invalid. Please configure game path in settings.",
                                  "Game Path Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _gameProcessService.StartGame(account, _gameConfig.GamePath, (success) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (success)
                        {
                            account.GameStatus = "Game started, logging in...";

                            // Gửi LOGIN message theo đúng format tới client
                            var loginMsg = ConvertMsg("LOGIN", account.Username, account.Password, account.IndexCharacter.ToString(), account.IndexServer.ToString());
                            _gameProcessService.SendCommandToPipe(account.Id, loginMsg);
                        }
                        else
                        {
                            account.GameStatus = "Failed to start game";
                        }

                        OnPropertyChanged(nameof(HasRunningGames));
                        OnPropertyChanged(nameof(CanUseAutoControls));
                        OnPropertyChanged(nameof(IsConnectionStatusVisible));
                        OnPropertyChanged(nameof(ConnectionStatusText));
                    }));
                });
            }
        }

        private void ExecuteCheckGameConnection(Account account)
        {
            if (account == null || !account.IsGameRunning) return;

            _gameProcessService.CheckGameConnection(account);
            OnPropertyChanged(nameof(CanUseAutoControls));
            OnPropertyChanged(nameof(IsConnectionStatusVisible));
            OnPropertyChanged(nameof(ConnectionStatusText));
        }

        private void ExecuteToggleAutoState()
        {
            if (_selectedAccount?.AutoSettings != null)
            {
                _selectedAccount.AutoSettings.AutoState = !_selectedAccount.AutoSettings.AutoState;
                SendAutoCommand("AUTO_STATE", _selectedAccount.AutoSettings.AutoState.ToString());
            }
        }

        private void ExecuteToggleAutoEvent()
        {
            if (_selectedAccount?.AutoSettings != null)
            {
                _selectedAccount.AutoSettings.AutoEvent = !_selectedAccount.AutoSettings.AutoEvent;
                SendAutoCommand("AUTO_EVENT", _selectedAccount.AutoSettings.AutoEvent.ToString());
            }
        }

        private void ExecuteToggleAutoEquip()
        {
            if (_selectedAccount?.AutoSettings != null)
            {
                _selectedAccount.AutoSettings.AutoEquip = !_selectedAccount.AutoSettings.AutoEquip;
                SendAutoCommand("AUTO_EQUIP", _selectedAccount.AutoSettings.AutoEquip.ToString());
            }
        }

        private void SendAutoCommand(string command, string value)
        {
            if (_selectedAccount != null && _selectedAccount.CanUseGameControls)
            {
                var fullCommand = ConvertMsg(command, value);
                _gameProcessService.SendCommandToPipe(_selectedAccount.Id, fullCommand);
            }
        }

        // Event handler for character name received from game
        private void OnCharacterNameReceived(Account account, string characterName)
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

        #endregion

        #region Private Methods
        private string ConvertMsg(params string[] parts)
        {
            return string.Join("|", parts);
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
