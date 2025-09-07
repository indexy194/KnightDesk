using System;
using System.Collections.Generic;
using KnightDesk.Presentation.WPF.DTOs;
using KnightDesk.Presentation.WPF.Models;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface IAccountApiService
    {
        void GetAllAccountsAsync(Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback);
        void GetAccountByIdAsync(int id, Action<GeneralResponseDTO<AccountDTO>> callback);
        void GetAccountsByUserIdAsync(int userId, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback);
        void SearchAccountsAsync(string searchText, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback);
        void AddAccountAsync(CreateAccountDTO account, Action<GeneralResponseDTO<AccountDTO>> callback);
        void UpdateAccountAsync(UpdateAccountDTO account, Action<GeneralResponseDTO<AccountDTO>> callback);
        void DeleteAccountAsync(int id, Action<GeneralResponseDTO<bool>> callback);
        void ToggleFavoriteAsync(int id, Action<GeneralResponseDTO<bool>> callback);
        void GetFavoriteAccountsAsync(Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback);
        void GetFavoriteAccountsByUserIdAsync(int userId, Action<GeneralResponseDTO<IEnumerable<AccountDTO>>> callback);
    }
}
