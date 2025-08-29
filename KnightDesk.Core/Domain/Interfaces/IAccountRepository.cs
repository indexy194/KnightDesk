using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Domain.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountsByServerAsync(int serverNo);
        Task<IEnumerable<Account>> GetFavoriteAccountsAsync();
        Task<Account?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameExistsAsync(string username, int serverNo);
        Task<IEnumerable<Account>> SearchAccountsAsync(string searchTerm);
        Task ToggleFavoriteAsync(int accountId);
        Task<IEnumerable<Account>> GetListAccountsByUserId(int userId);
    }
}
