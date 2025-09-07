using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class CharacterIndexOption
    {
        public int Value { get; set; }
        public string DisplayText { get; set; }
    }

    public class AccountPageViewModel : INotifyPropertyChanged
    {
        private readonly IAccountApiService _accountService;
        private readonly IServerInfoService _serverInfoService;
        
        private ObservableCollection<AccountDTO> _accounts;
        private ObservableCollection<AccountDTO> _filteredAccounts;
        private ObservableCollection<ServerInfo> _servers;
        private AccountDTO _currentAccount;
        private ServerInfo _selectedServer;
        private string _searchText;
        private string _formTitle;
        private string _saveButtonText;
        private bool _isEditMode;
        private string _currentPassword;
        private ObservableCollection<CharacterIndexOption> _characterIndexOptions;
        private CharacterIndexOption _selectedCharacterIndex;

        public AccountPageViewModel()
        {
            _accountService = new AccountApiService();
            _serverInfoService = new ServerInfoService();
            
            // Initialize collections
            _accounts = new ObservableCollection<AccountDTO>();
            _filteredAccounts = new ObservableCollection<AccountDTO>();
            _servers = new ObservableCollection<ServerInfo>();
            _characterIndexOptions = new ObservableCollection<CharacterIndexOption>
            {
                new CharacterIndexOption { Value = 1, DisplayText = "Nhân vật 1" },
                new CharacterIndexOption { Value = 2, DisplayText = "Nhân vật 2" },
                new CharacterIndexOption { Value = 3, DisplayText = "Nhân vật 3" }
            }; 
            
            // Initialize commands
            AddNewAccountCommand = new RelayCommand(ExecuteAddNewAccount);
            EditAccountCommand = new RelayCommand<object>(ExecuteEditAccount);
            DeleteAccountCommand = new RelayCommand<object>(ExecuteDeleteAccount);
            ToggleFavoriteCommand = new RelayCommand<object>(ExecuteToggleFavorite);
            SaveAccountCommand = new RelayCommand(ExecuteSaveAccount, CanExecuteSaveAccount);
            CancelCommand = new RelayCommand(ExecuteCancel);

            // Initialize form
            ResetForm();
            
            // Load initial data
            LoadAccounts();
            LoadServers();
        }

        #region Properties

        public ObservableCollection<AccountDTO> Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                OnPropertyChanged("Accounts");
            }
        }

        public ObservableCollection<AccountDTO> FilteredAccounts
        {
            get { return _filteredAccounts; }
            set
            {
                _filteredAccounts = value;
                OnPropertyChanged("FilteredAccounts");
            }
        }

        public ObservableCollection<ServerInfo> Servers
        {
            get { return _servers; }
            set
            {
                _servers = value;
                OnPropertyChanged("Servers");
            }
        }

        public AccountDTO CurrentAccount
        {
            get { return _currentAccount; }
            set
            {
                _currentAccount = value;
                OnPropertyChanged("CurrentAccount");
            }
        }

        public ServerInfo SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                _selectedServer = value;
                OnPropertyChanged("SelectedServer");
                OnPropertyChanged("CanSave");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                FilterAccounts();
            }
        }

        public string FormTitle
        {
            get { return _formTitle; }
            set
            {
                _formTitle = value;
                OnPropertyChanged("FormTitle");
            }
        }

        public string SaveButtonText
        {
            get { return _saveButtonText; }
            set
            {
                _saveButtonText = value;
                OnPropertyChanged("SaveButtonText");
            }
        }

        public string CurrentPassword
        {
            get { return _currentPassword; }
            set
            {
                _currentPassword = value;
                OnPropertyChanged("CurrentPassword");
                OnPropertyChanged("CanSave");
            }
        }

        public bool CanSave
        {
            get 
            { 
                return _currentAccount != null && 
                       !string.IsNullOrEmpty(_currentAccount.Username) && 
                       !string.IsNullOrEmpty(_currentPassword) &&
                       _selectedServer != null &&
                       _selectedCharacterIndex != null;
            }
        }

        public ObservableCollection<CharacterIndexOption> CharacterIndexOptions
        {
            get { return _characterIndexOptions; }
            set
            {
                _characterIndexOptions = value;
                OnPropertyChanged("CharacterIndexOptions");
            }
        }

        public CharacterIndexOption SelectedCharacterIndex
        {
            get { return _selectedCharacterIndex; }
            set
            {
                _selectedCharacterIndex = value;
                OnPropertyChanged("SelectedCharacterIndex");
                OnPropertyChanged("CanSave");
                if (_currentAccount != null && value != null)
                {
                    _currentAccount.IndexCharacter = value.Value;
                }
            }
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                _isEditMode = value;
                OnPropertyChanged("IsEditMode");
                OnPropertyChanged("IsCharacterNameEnabled");
            }
        }

        public bool IsCharacterNameEnabled
        {
            get { return _isEditMode; }
        }

        #endregion

        #region Commands

        public ICommand AddNewAccountCommand { get; private set; }
        public ICommand EditAccountCommand { get; private set; }
        public ICommand DeleteAccountCommand { get; private set; }
        public ICommand ToggleFavoriteCommand { get; private set; }
        public ICommand SaveAccountCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        #endregion

        #region Command Implementations

        //private void ExecuteSearch()
        //{
        //    if (!string.IsNullOrEmpty(_searchText))
        //    {
        //        _accountService.SearchAccountsAsync(_searchText, new Action<GeneralResponseDTO<IEnumerable<AccountDTO>>>(result =>
        //        {
        //            Application.Current.Dispatcher.Invoke(new Action(() =>
        //            {
        //                if (result.Code == 200 && result.Data != null)
        //                {
        //                    _accounts.Clear();
        //                    foreach (var account in result.Data)
        //                    {
        //                        _accounts.Add(account);
        //                    }
        //                    FilterAccounts();
        //                }
        //            }));
        //        }));
        //    }
        //    else
        //    {
        //        LoadAccounts();
        //    }
        //}

        private void ExecuteAddNewAccount()
        {
            ResetForm();
            IsEditMode = false;
            FormTitle = "Add New Account";
            SaveButtonText = "Add";
        }

        private void ExecuteEditAccount(object parameter)
        {
            if (parameter is AccountDTO account)
            {
                IsEditMode = true;
                CurrentAccount = new AccountDTO
                {
                    Id = account.Id,
                    Username = account.Username,
                    CharacterName = account.CharacterName,
                    IndexCharacter = account.IndexCharacter,
                    ServerInfoId = account.ServerInfo.Id,
                    IsFavorite = account.IsFavorite
                };
                CurrentPassword = account.Password;
                SelectedServer = _servers.FirstOrDefault(s => s.Id == account.ServerInfo.Id);
                SelectedCharacterIndex = _characterIndexOptions.FirstOrDefault(c => c.Value == account.IndexCharacter);
                FormTitle = "Edit Account";
                SaveButtonText = "Update";
            }
        }

        private void ExecuteDeleteAccount(object parameter)
        {
            if (parameter is AccountDTO account)
            {
                var result = MessageBox.Show(
                    string.Format("Are you sure you want to delete account '{0}'?", account.Username),
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _accountService.DeleteAccountAsync(account.Id, new Action<GeneralResponseDTO<bool>>(deleteResult =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (deleteResult.Code == 200 && deleteResult.Data)
                            {
                                LoadAccounts();
                                MessageBox.Show("Account deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete account: " + deleteResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }));
                    }));
                }
            }
        }

        private void ExecuteToggleFavorite(object parameter)
        {
            if (parameter is AccountDTO account)
            {
                _accountService.ToggleFavoriteAsync(account.Id, new Action<GeneralResponseDTO<bool>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == 200 && result.Data)
                        {
                            // Update the account in the local collections
                            var accountInList = _accounts.FirstOrDefault(a => a.Id == account.Id);
                            if (accountInList != null)
                            {
                                accountInList.IsFavorite = !accountInList.IsFavorite;
                            }
                            
                            var accountInFiltered = _filteredAccounts.FirstOrDefault(a => a.Id == account.Id);
                            if (accountInFiltered != null)
                            {
                                accountInFiltered.IsFavorite = !accountInFiltered.IsFavorite;
                            }
                            
                            // Also update the parameter account
                            account.IsFavorite = !account.IsFavorite;
                        }
                        else
                        {
                            MessageBox.Show("Failed to toggle favorite: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
                }));
            }
        }

        private void ExecuteSaveAccount()
        {
            if (!CanExecuteSaveAccount())
                return;

            if (_isEditMode)
            {
                var updateDto = new UpdateAccountDTO
                {
                    Id = _currentAccount.Id,
                    UserId = Properties.Settings.Default.UserId,
                    Username = _currentAccount.Username,
                    Password = _currentPassword,
                    CharacterName = _currentAccount.CharacterName,
                    IndexCharacter = _selectedCharacterIndex.Value,
                    ServerInfoId = _selectedServer.Id,
                    IsFavorite = _currentAccount.IsFavorite
                };

                _accountService.UpdateAccountAsync(updateDto, new Action<GeneralResponseDTO<AccountDTO>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == 200 && result.Data != null)
                        {
                            LoadAccounts();
                            ResetForm();
                            MessageBox.Show("Account updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update account: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
                }));
            }
            else
            {
                var createDto = new CreateAccountDTO
                {
                    UserId = Properties.Settings.Default.UserId,
                    Username = _currentAccount.Username,
                    Password = _currentPassword,
                    IndexCharacter = _selectedCharacterIndex.Value,
                    ServerInfoId = _selectedServer.Id,
                    IsFavorite = _currentAccount.IsFavorite
                };

                _accountService.AddAccountAsync(createDto, new Action<GeneralResponseDTO<AccountDTO>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == 200 && result.Data != null)
                        {
                            LoadAccounts();
                            ResetForm();
                            MessageBox.Show("Account added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to add account: " + result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }));
                }));
            }
        }

        private bool CanExecuteSaveAccount()
        {
            return CanSave;
        }

        private void ExecuteCancel()
        {
            ResetForm();
        }

        #endregion

        #region Private Methods

        private AccountDTO FindAccountById(int accountId)
        {
            return _accounts.FirstOrDefault(a => a.Id == accountId);
        }

        private void LoadAccounts()
        {
            var userId = Properties.Settings.Default.UserId;
            if (userId > 0)
            {
                _accountService.GetAccountsByUserIdAsync(userId, new Action<GeneralResponseDTO<IEnumerable<AccountDTO>>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == 200 && result.Data != null)
                        {
                            _accounts.Clear();
                            foreach (var account in result.Data)
                            {
                                _accounts.Add(account);
                            }
                            FilterAccounts();
                        }
                    }));
                }));
            }
        }

        private void LoadServers()
        {
            _serverInfoService.GetAllServersAsync(new Action<List<ServerInfo>>(servers =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    _servers.Clear();
                    foreach (var server in servers)
                    {
                        _servers.Add(server);
                    }
                }));
            }));
        }

        private void FilterAccounts()
        {
            _filteredAccounts.Clear();
            
            if (string.IsNullOrEmpty(_searchText))
            {
                foreach (var account in _accounts)
                {
                    _filteredAccounts.Add(account);
                }
            }
            else
            {
                var searchLower = _searchText.ToLower();
                foreach (var account in _accounts)
                {
                    if (account.Username.ToLower().Contains(searchLower) ||
                        account.CharacterName.ToLower().Contains(searchLower))
                    {
                        _filteredAccounts.Add(account);
                    }
                }
            }
        }

        private void ResetForm()
        {
            CurrentAccount = new AccountDTO();
            SelectedServer = null;
            SelectedCharacterIndex = null;
            CurrentPassword = string.Empty;
            IsEditMode = false;
            FormTitle = "Add New Account";
            SaveButtonText = "Add";
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
