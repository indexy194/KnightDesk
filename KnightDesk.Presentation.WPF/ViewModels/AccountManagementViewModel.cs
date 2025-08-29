using KnightDesk.Presentation.WPF.Models;
using KnightDesk.Presentation.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KnightDesk.Presentation.WPF.ViewModels
{
    public class AccountManagementViewModel : BaseViewModel
    {
        //private readonly IAccountApiService _accountApiService;

        //private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();
        //private string _searchText = string.Empty;
        //private Account _selectedAccount;
        //private bool _isLoading;

        //public AccountManagementViewModel(IAccountApiService accountApiService)
        //{
        //    _accountApiService = accountApiService;

        //    // Commands
        //    LoadAccountsCommand = new RelayCommand(async () => await LoadAccountsAsync());
        //    SearchCommand = new RelayCommand(async () => await SearchAccountsAsync());
        //    ToggleFavoriteCommand = new RelayCommand<Account>(async (account) => await ToggleFavoriteAsync(account));
        //    RefreshCommand = new RelayCommand(async () => await RefreshAsync());

        //    // Load initial data
        //    _ = LoadAccountsAsync();
        //}

        //#region Properties

        //public ObservableCollection<Account> Accounts
        //{
        //    get => _accounts;
        //    set => SetProperty(ref _accounts, value);
        //}

        //public string SearchText
        //{
        //    get => _searchText;
        //    set
        //    {
        //        if (SetProperty(ref _searchText, value))
        //        {
        //            // Auto-search when text changes (debounced)
        //            _ = SearchAccountsAsync();
        //        }
        //    }
        //}

        //public Account SelectedAccount
        //{
        //    get => _selectedAccount;
        //    set => SetProperty(ref _selectedAccount, value);
        //}

        //public bool IsLoading
        //{
        //    get => _isLoading;
        //    set => SetProperty(ref _isLoading, value);
        //}

        //#endregion

        //#region Commands

        //public ICommand LoadAccountsCommand { get; }
        //public ICommand SearchCommand { get; }
        //public ICommand ToggleFavoriteCommand { get; }
        //public ICommand RefreshCommand { get; }

        //#endregion

        //#region Methods

        //private async Task LoadAccountsAsync()
        //{
        //    try
        //    {
        //        IsLoading = true;
        //        var accounts = await _accountApiService.GetAllAccountsAsync();
                
        //        Accounts.Clear();
        //        foreach (var account in accounts)
        //        {
        //            Accounts.Add(account);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle error (show message to user)
        //        System.Diagnostics.Debug.WriteLine($"Error loading accounts: {ex.Message}");
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //    }
        //}

        //private async Task SearchAccountsAsync()
        //{
        //    try
        //    {
        //        IsLoading = true;
        //        var accounts = await _accountApiService.SearchAccountsAsync(SearchText);
                
        //        Accounts.Clear();
        //        foreach (var account in accounts)
        //        {
        //            Accounts.Add(account);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error searching accounts: {ex.Message}");
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //    }
        //}

        //private async Task ToggleFavoriteAsync(Account account)
        //{
        //    if (account == null) return;

        //    try
        //    {
        //        var updatedAccount = await _accountApiService.ToggleFavoriteAsync(account.Id);
        //        if (updatedAccount != null)
        //        {
        //            account.IsFavorite = updatedAccount.IsFavorite;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error toggling favorite: {ex.Message}");
        //    }
        //}

        //private async Task RefreshAsync()
        //{
        //    SearchText = string.Empty;
        //    await LoadAccountsAsync();
        //}

        //#endregion
    }
}
