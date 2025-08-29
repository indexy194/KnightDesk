using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KnightDesk.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                .Include(u => u.Accounts)
                .ThenInclude(a => a.ServerInfo)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetByIPAddressAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return null;

            return await _context.Users
                .Include(u => u.Accounts)
                .ThenInclude(a => a.ServerInfo)
                .FirstOrDefaultAsync(u => u.IPAddress == ipAddress && !u.IsDeleted);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<bool> ExistsByIPAddressAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            return await _context.Users
                .AnyAsync(u => u.IPAddress == ipAddress && !u.IsDeleted);
        }

        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            // Note: In production, you should hash the password before comparing
            // This is a simplified version for demonstration
            return await _context.Users
                .Include(u => u.Accounts)
                .ThenInclude(a => a.ServerInfo)
                .FirstOrDefaultAsync(u => u.Username == username && 
                                        u.Password == password && 
                                        !u.IsDeleted);
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Accounts)
                .ThenInclude(a => a.ServerInfo)
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Accounts)
                .ThenInclude(a => a.ServerInfo)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}
