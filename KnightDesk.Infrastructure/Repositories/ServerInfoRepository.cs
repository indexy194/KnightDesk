using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KnightDesk.Infrastructure.Repositories
{
    public class ServerInfoRepository : Repository<ServerInfo>, IServerInfoRepository
    {
        public ServerInfoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ServerInfo?> GetByIndexServerAsync(int indexServer)
        {
            return await _dbSet
                .Include(s => s.Accounts)
                .FirstOrDefaultAsync(s => s.IndexServer == indexServer && !s.IsDeleted);
        }

        public async Task<IEnumerable<ServerInfo>> GetActiveServersAsync()
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.IndexServer)
                .ToListAsync();
        }

        public async Task<bool> IsIndexServerExistsAsync(int indexServer)
        {
            return await _dbSet
                .AnyAsync(s => s.IndexServer == indexServer && !s.IsDeleted);
        }

        public override async Task<IEnumerable<ServerInfo>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Accounts)
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.IndexServer)
                .ToListAsync();
        }

        public override async Task<ServerInfo?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Accounts)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public Task<ServerInfo> GetServerInfoByIndexServerAsync(int indexServer)
        {
            return _dbSet
                .Include(s => s.Accounts)
                .FirstAsync(s => s.IndexServer == indexServer && !s.IsDeleted);
        }
    }
}
