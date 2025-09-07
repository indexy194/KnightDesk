using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels.Pages
{
    public class ManagerPageViewModel : INotifyPropertyChanged
    {
        private readonly IAccountApiService _accountService;
        private List<AccountDTO> _allAccounts;
        private List<AccountDTO> _filteredAccounts;
        private AccountDTO _selectedAccount;
        private string _searchText;
        private bool _isLoading;

        public ManagerPageViewModel()
        {
            _accountService = new AccountApiService();
            _allAccounts = new List<AccountDTO>();
            _filteredAccounts = new List<AccountDTO>();

            // Initialize commands
            SearchCommand = new RelayCommand(new Action(ExecuteSearch));
            ToggleFavoriteCommand = new RelayCommand<object>(ExecuteToggleFavorite);
            SelectAccountCommand = new RelayCommand<object>(ExecuteSelectAccount);

            // Load favorite accounts on initialization
            LoadFavoriteAccounts();
        }

        #region Properties

        public List<AccountDTO> FilteredAccounts
        {
            get { return _filteredAccounts; }
            set
            {
                _filteredAccounts = value;
                OnPropertyChanged("FilteredAccounts");
            }
        }

        public AccountDTO SelectedAccount
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
        public ICommand ToggleFavoriteCommand { get; private set; }
        public ICommand SelectAccountCommand { get; private set; }

        #endregion

        #region Command Implementations

        private void ExecuteSearch()
        {
            if (string.IsNullOrEmpty(_searchText))
            {
                FilteredAccounts = new List<AccountDTO>(_allAccounts);
            }
            else
            {
                var searchLower = _searchText.ToLower();
                FilteredAccounts = _allAccounts.Where(account =>
                    (!string.IsNullOrEmpty(account.Username) && account.Username.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(account.CharacterName) && account.CharacterName.ToLower().Contains(searchLower))
                ).ToList();
            }
        }

        private void ExecuteToggleFavorite(object parameter)
        {
            if (parameter is AccountDTO account)
            {
                IsLoading = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
                {
                    _accountService.ToggleFavoriteAsync(account.Id, new Action<GeneralResponseDTO<bool>>(result =>
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (result.Code == 200)
                            {
                                // Update the account in the list
                                var accountToUpdate = _allAccounts.FirstOrDefault(a => a.Id == account.Id);
                                if (accountToUpdate != null)
                                {
                                    accountToUpdate.IsFavorite = !accountToUpdate.IsFavorite;
                                    ExecuteSearch(); // Refresh the filtered list
                                }
                            }
                            IsLoading = false;
                        }));
                    }));
                }));
            }
        }

        private void ExecuteSelectAccount(object parameter)
        {
            if (parameter is AccountDTO account)
            {
                SelectedAccount = account;
            }
        }

        #endregion

        #region Private Methods

        private void LoadFavoriteAccounts()
        {
            IsLoading = true;

            ThreadPool.QueueUserWorkItem(new WaitCallback(state =>
            {
                int userId = Properties.Settings.Default.UserId;

                // Use the correct API endpoint for getting favorite accounts by user ID
                _accountService.GetFavoriteAccountsByUserIdAsync(userId, new Action<GeneralResponseDTO<IEnumerable<AccountDTO>>>(result =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (result.Code == 200 && result.Data != null)
                        {
                            _allAccounts = result.Data.ToList();
                            FilteredAccounts = new List<AccountDTO>(_allAccounts);

                            // Set first account as selected if available
                            if (_allAccounts.Count > 0)
                            {
                                SelectedAccount = _allAccounts.First();
                            }
                        }
                        IsLoading = false;
                    }));
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
