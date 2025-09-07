using KnightDesk.Core.Application.DTOs;

namespace KnightDesk.Core.Application.Services
{
    public interface IAccountService
    {
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetAllAccountsAsync();
        Task<GeneralResponseDTO<AccountDTO?>> GetAccountByIdAsync(int id);
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetAccountsByServerAsync(int serverNo);
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetFavoriteAccountsAsync();
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> SearchAccountsAsync(string searchTerm);
        Task<GeneralResponseDTO<AccountDTO>> CreateAccountAsync(CreateAccountDTO account);
        Task<GeneralResponseDTO<AccountDTO>> UpdateAccountAsync(UpdateAccountDTO account);
        Task<GeneralResponseDTO<bool>> DeleteAccountAsync(int id);
        Task<GeneralResponseDTO<bool>> ToggleFavoriteAsync(int accountId);
        Task<GeneralResponseDTO<bool>> IsUsernameExistsAsync(string username);
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetListAccountsByUserId(int userId);
        //get account favorite by user id
        Task<GeneralResponseDTO<IEnumerable<AccountDTO>>> GetFavoriteAccountsByUserId(int userId);
    }
}
