using System;
using System.Collections.Generic;
using KnightDesk.Presentation.WPF.Models;

namespace KnightDesk.Presentation.WPF.Services
{
    public class AccountApiService  // : IAccountApiService
    {
        //private readonly HttpClient _httpClient;
        //private readonly string _baseUrl;

        //public AccountApiService(string baseUrl = "https://localhost:7001")
        //{
        //    _httpClient = new HttpClient();
        //    _baseUrl = baseUrl;
        //}

        //public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_baseUrl}/api/accounts");
        //        response.EnsureSuccessStatusCode();
                
        //        var json = await response.Content.ReadAsStringAsync();
        //        var accounts = JsonConvert.DeserializeObject<List<Account>>(json);
                
        //        return accounts ?? new List<Account>();
        //    }
        //    catch (Exception)
        //    {
        //        // Log error in production
        //        return new List<Account>();
        //    }
        //}

        //public async Task<Account> GetAccountByIdAsync(int id)
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_baseUrl}/api/accounts/{id}");
        //        response.EnsureSuccessStatusCode();
                
        //        var json = await response.Content.ReadAsStringAsync();
        //        var account = JsonConvert.DeserializeObject<Account>(json);
                
        //        return account;
        //    }
        //    catch (Exception)
        //    {
        //        // Log error in production
        //        return null;
        //    }
        //}

        //public async Task<IEnumerable<Account>> SearchAccountsAsync(string searchText)
        //{
        //    try
        //    {
        //        var encodedSearchText = Uri.EscapeDataString(searchText ?? string.Empty);
        //        var response = await _httpClient.GetAsync($"{_baseUrl}/api/accounts/search?searchText={encodedSearchText}");
        //        response.EnsureSuccessStatusCode();
                
        //        var json = await response.Content.ReadAsStringAsync();
        //        var accounts = JsonConvert.DeserializeObject<List<Account>>(json);
                
        //        return accounts ?? new List<Account>();
        //    }
        //    catch (Exception)
        //    {
        //        // Log error in production
        //        return new List<Account>();
        //    }
        //}

        //public async Task<Account> ToggleFavoriteAsync(int id)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PutAsync($"{_baseUrl}/api/accounts/{id}/toggle-favorite", null);
        //        response.EnsureSuccessStatusCode();
                
        //        var json = await response.Content.ReadAsStringAsync();
        //        var account = JsonConvert.DeserializeObject<Account>(json);
                
        //        return account;
        //    }
        //    catch (Exception)
        //    {
        //        // Log error in production
        //        return null;
        //    }
        //}

        //public async Task<IEnumerable<Account>> GetFavoriteAccountsAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_baseUrl}/api/accounts/favorites");
        //        response.EnsureSuccessStatusCode();
                
        //        var json = await response.Content.ReadAsStringAsync();
        //        var accounts = JsonConvert.DeserializeObject<List<Account>>(json);
                
        //        return accounts ?? new List<Account>();
        //    }
        //    catch (Exception)
        //    {
        //        // Log error in production
        //        return new List<Account>();
        //    }
        //}

        //public void Dispose()
        //{
        //    _httpClient?.Dispose();
        //}
    }
}
