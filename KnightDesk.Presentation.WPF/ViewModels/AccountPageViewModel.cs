using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels
{
    public class AccountPageViewModel //: INotifyPropertyChanged
    {
        //private readonly IAccountApiService _accountService;
        //private ObservableCollection<Account> _accounts;
        //private ObservableCollection<Account> _filteredAccounts;
        //private ObservableCollection<ServerInfo> _servers;
        //private Account _currentAccount;
        //private ServerInfo _selectedServer;
        //private string _searchText = string.Empty;
        //private bool _isEditMode = false;

        //public AccountPageViewModel()
        //{
        //    _accountService = new AccountApiService();
            
        //    Accounts = new ObservableCollection<Account>();
        //    FilteredAccounts = new ObservableCollection<Account>();
        //    Servers = new ObservableCollection<ServerInfo>();
        //    CurrentAccount = new Account();

        //    // Initialize commands - .NET 3.5 compatible
        //    SearchCommand = new RelayCommand(new Action(ExecuteSearch));
        //    AddNewAccountCommand = new RelayCommand(new Action(ExecuteAddNew));
        //    EditAccountCommand = new RelayCommand<Account>(new Action<Account>(ExecuteEdit));
        //    DeleteAccountCommand = new RelayCommand<Account>(new Action<Account>(ExecuteDelete));
        //    SaveAccountCommand = new RelayCommand(new Action(ExecuteSave), new Func<bool>(CanExecuteSave));
        //    CancelCommand = new RelayCommand(new Action(ExecuteCancel));
        //    ToggleFavoriteCommand = new RelayCommand<Account>(new Action<Account>(ExecuteToggleFavorite));

        //    LoadData();
        //}

        //#region Properties

        //public ObservableCollection<Account> Accounts
        //{
        //    get { return _accounts; }
        //    set
        //    {
        //        _accounts = value;
        //        OnPropertyChanged("Accounts");
        //    }
        //}

        //public ObservableCollection<Account> FilteredAccounts
        //{
        //    get { return _filteredAccounts; }
        //    set
        //    {
        //        _filteredAccounts = value;
        //        OnPropertyChanged("FilteredAccounts");
        //    }
        //}

        //public ObservableCollection<ServerInfo> Servers
        //{
        //    get { return _servers; }
        //    set
        //    {
        //        _servers = value;
        //        OnPropertyChanged("Servers");
        //    }
        //}

        //public Account CurrentAccount
        //{
        //    get { return _currentAccount; }
        //    set
        //    {
        //        _currentAccount = value;
        //        OnPropertyChanged("CurrentAccount");
        //        OnPropertyChanged("FormTitle");
        //        OnPropertyChanged("SaveButtonText");
        //    }
        //}

        //public ServerInfo SelectedServer
        //{
        //    get { return _selectedServer; }
        //    set
        //    {
        //        _selectedServer = value;
        //        if (CurrentAccount != null && value != null)
        //        {
        //            CurrentAccount.ServerInfoId = value.Id;
        //        }
        //        OnPropertyChanged("SelectedServer");
        //    }
        //}

        //public string SearchText
        //{
        //    get { return _searchText; }
        //    set
        //    {
        //        _searchText = value;
        //        OnPropertyChanged("SearchText");
        //        FilterAccounts();
        //    }
        //}

        //public string FormTitle 
        //{ 
        //    get { return _isEditMode ? "Edit Account" : "Add New Account"; } 
        //}

        //public string SaveButtonText 
        //{ 
        //    get { return _isEditMode ? "Update" : "Save"; } 
        //}

        //#endregion

        //#region Commands

        //public ICommand SearchCommand { get; private set; }
        //public ICommand AddNewAccountCommand { get; private set; }
        //public ICommand EditAccountCommand { get; private set; }
        //public ICommand DeleteAccountCommand { get; private set; }
        //public ICommand SaveAccountCommand { get; private set; }
        //public ICommand CancelCommand { get; private set; }
        //public ICommand ToggleFavoriteCommand { get; private set; }

        //#endregion

        //#region Command Implementations

        //private void ExecuteSearch()
        //{
        //    FilterAccounts();
        //}

        //private void ExecuteAddNew()
        //{
        //    _isEditMode = false;
        //    CurrentAccount = new Account();
        //    SelectedServer = null;
        //    OnPropertyChanged("FormTitle");
        //    OnPropertyChanged("SaveButtonText");
        //}

        //private void ExecuteEdit(Account account)
        //{
        //    if (account != null)
        //    {
        //        _isEditMode = true;
        //        CurrentAccount = new Account
        //        {
        //            Id = account.Id,
        //            Username = account.Username,
        //            CharacterName = account.CharacterName,
        //            Password = account.Password,
        //            IndexCharacter = account.IndexCharacter,
        //            ServerInfoId = account.ServerInfoId,
        //            IsFavorite = account.IsFavorite,
        //            UserId = account.UserId
        //        };
                
        //        SelectedServer = Servers.FirstOrDefault(delegate(ServerInfo s) { return s.Id == account.ServerInfoId; });
        //        OnPropertyChanged("FormTitle");
        //        OnPropertyChanged("SaveButtonText");
        //    }
        //}

        //private void ExecuteDelete(Account account)
        //{
        //    if (account != null)
        //    {
        //        MessageBoxResult result = MessageBox.Show(
        //            string.Format("Are you sure you want to delete the account '{0}'?", account.Username), 
        //            "Confirm Delete", 
        //            MessageBoxButton.YesNo, 
        //            MessageBoxImage.Question);
                
        //        if (result == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                // For now, just remove from collection - replace with actual API call
        //                Accounts.Remove(account);
        //                FilterAccounts();
        //                MessageBox.Show("Account deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(string.Format("Error deleting account: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //}

        //private void ExecuteSave()
        //{
        //    try
        //    {
        //        if (_isEditMode)
        //        {
        //            // Update existing account
        //            Account existingAccount = Accounts.FirstOrDefault(delegate(Account a) { return a.Id == CurrentAccount.Id; });
        //            if (existingAccount != null)
        //            {
        //                int index = Accounts.IndexOf(existingAccount);
        //                Accounts[index] = CurrentAccount;
        //            }
        //            MessageBox.Show("Account updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        else
        //        {
        //            // Add new account
        //            CurrentAccount.Id = Accounts.Count > 0 ? Accounts.Max(delegate(Account a) { return a.Id; }) + 1 : 1;
        //            CurrentAccount.ServerInfo = SelectedServer;
        //            Accounts.Add(CurrentAccount);
        //            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }

        //        FilterAccounts();
        //        ExecuteCancel();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(string.Format("Error saving account: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private bool CanExecuteSave()
        //{
        //    return CurrentAccount != null && 
        //           !string.IsNullOrEmpty(CurrentAccount.Username) && 
        //           !string.IsNullOrEmpty(CurrentAccount.CharacterName) &&
        //           !string.IsNullOrEmpty(CurrentAccount.Password) &&
        //           SelectedServer != null;
        //}

        //private void ExecuteCancel()
        //{
        //    _isEditMode = false;
        //    CurrentAccount = new Account();
        //    SelectedServer = null;
        //    OnPropertyChanged("FormTitle");
        //    OnPropertyChanged("SaveButtonText");
        //}

        //private void ExecuteToggleFavorite(Account account)
        //{
        //    if (account != null)
        //    {
        //        try
        //        {
        //            account.IsFavorite = !account.IsFavorite;
        //            // In real implementation, call API here
        //        }
        //        catch (Exception ex)
        //        {
        //            account.IsFavorite = !account.IsFavorite; // Revert on error
        //            MessageBox.Show(string.Format("Error updating favorite status: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

        //#endregion

        //#region Private Methods

        //private void LoadData()
        //{
        //    try
        //    {
        //        // Mock data for now - replace with actual API calls
        //        Accounts.Clear();
        //        Accounts.Add(new Account { Id = 1, Username = "player123@gmail.com", CharacterName = "Knight123", Password = "password123", IndexCharacter = 1, ServerInfoId = 1, IsFavorite = false });
        //        Accounts.Add(new Account { Id = 2, Username = "gamer456@outlook.com", CharacterName = "ProGamer", Password = "password456", IndexCharacter = 2, ServerInfoId = 2, IsFavorite = false });
        //        Accounts.Add(new Account { Id = 3, Username = "prouser789@yahoo.com", CharacterName = "ElitePlayer", Password = "password789", IndexCharacter = 3, ServerInfoId = 1, IsFavorite = true });

        //        LoadServers();
        //        FilterAccounts();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(string.Format("Error loading data: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void LoadServers()
        //{
        //    // Mock server data - replace with actual API call when available
        //    Servers.Clear();
        //    Servers.Add(new ServerInfo { Id = 1, Name = "Server 1", IndexServer = 1 });
        //    Servers.Add(new ServerInfo { Id = 2, Name = "Server 2", IndexServer = 2 });
        //    Servers.Add(new ServerInfo { Id = 3, Name = "Server 3", IndexServer = 3 });
        //}

        //private void FilterAccounts()
        //{
        //    FilteredAccounts.Clear();
            
        //    if (string.IsNullOrEmpty(SearchText))
        //    {
        //        foreach (Account account in Accounts)
        //        {
        //            FilteredAccounts.Add(account);
        //        }
        //    }
        //    else
        //    {
        //        foreach (Account account in Accounts)
        //        {
        //            if (account.Username.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
        //                account.CharacterName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
        //            {
        //                FilteredAccounts.Add(account);
        //            }
        //        }
        //    }
        //}

        //#endregion

        //#region INotifyPropertyChanged

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //#endregion
    }
}