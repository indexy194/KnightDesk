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
    public class ManagerPageViewModel : INotifyPropertyChanged
    {
        private readonly IAccountServices _accountService;
        private ObservableCollection<Account> _allAccounts;
        private ObservableCollection<Account> _filteredAccounts;
        private Account _selectedAccount;
        private string _searchText;
        private bool _isLoading;

        public ManagerPageViewModel()
        {
            _accountService = new AccountServices();
            _allAccounts = new ObservableCollection<Account>();
            _filteredAccounts = new ObservableCollection<Account>();

            // Initialize commands
            SearchCommand = new RelayCommand(ExecuteSearch);
            SelectAccountCommand = new RelayCommand<Account>(ExecuteSelectAccount);

            // Load favorite accounts on initialization
            LoadFavoriteAccounts();
        }

        #region Properties

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

        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; }
        public ICommand SelectAccountCommand { get; private set; }

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
                FilteredAccounts = new ObservableCollection<Account>(
                    _allAccounts.Where(account =>
                        (!string.IsNullOrEmpty(account.Username) && account.Username.ToLower().Contains(searchLower)) ||
                        (!string.IsNullOrEmpty(account.CharacterName) && account.CharacterName.ToLower().Contains(searchLower))
                    )
                );
            }
        }


        private void ExecuteSelectAccount(object parameter)
        {
            if (parameter is Account account)
            {
                SelectedAccount = account;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refresh the favorite accounts list - can be called from MainWindow
        /// </summary>
        public void RefreshFavoriteAccounts()
        {
            LoadFavoriteAccounts();
        }
        #endregion

        #region Private Methods

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
                        foreach(var entry in result.Data)
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
                            SelectedAccount = _allAccounts.First();
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
