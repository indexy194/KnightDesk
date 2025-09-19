using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KnightDesk.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Account>> GetAccountsByServerAsync(int serverNo)
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .Where(a => a.ServerInfo!.IndexServer == serverNo && !a.IsDeleted)
                .OrderBy(a => a.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetFavoriteAccountsAsync()
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .Where(a => a.IsFavorite && !a.IsDeleted)
                .OrderBy(a => a.Username)
                .ToListAsync();
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .FirstOrDefaultAsync(a => a.Username == username && !a.IsDeleted);
        }

        public async Task<bool> IsUsernameExistsAsync(string username, int userId)
        {
            return await _dbSet
                .AnyAsync(a => a.Username == username && a.UserId == userId;
        }

        public async Task<IEnumerable<Account>> SearchAccountsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var lowerSearchTerm = searchTerm.ToLower();
            return await _dbSet
                .Include(a => a.ServerInfo)
                .Where(a => !a.IsDeleted && (
                    a.Username!.ToLower().Contains(lowerSearchTerm) ||
                    a.CharacterName!.ToLower().Contains(lowerSearchTerm) ||
                    a.ServerInfo!.Name!.ToLower().Contains(lowerSearchTerm)
                ))
                .OrderBy(a => a.Username)
                .ToListAsync();
        }

        public async Task ToggleFavoriteAsync(int accountId)
        {
            var account = await GetByIdAsync(accountId);
            if (account != null)
            {
                account.IsFavorite = !account.IsFavorite;
                await UpdateAsync(account);
            }
        }

        public override async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.Username)
                .ToListAsync();
        }
        //get list account by user id
        public async Task<IEnumerable<Account>> GetListAccountsByUserId(int userId)
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .Where (a => !a.IsDeleted && a.UserId == userId)
                .OrderBy(a => a.Username)
                .ToListAsync();
        }

        public override async Task<Account?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(a => a.ServerInfo)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }
    }
}
