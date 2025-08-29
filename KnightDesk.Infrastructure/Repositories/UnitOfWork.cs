using KnightDesk.Core.Domain.Interfaces;
using KnightDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace KnightDesk.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        // Lazy initialization of repositories
        private IAccountRepository? _accountRepository;
        private IServerInfoRepository? _serverInfoRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IAccountRepository Accounts
        {
            get
            {
                return _accountRepository ??= new AccountRepository(_context);
            }
        }

        public IServerInfoRepository ServerInfos
        {
            get
            {
                return _serverInfoRepository ??= new ServerInfoRepository(_context);
            }
        }

        public IUserRepository Users
        {
            get
            {
                return _userRepository ??= new UserRepository(_context);
            }
        }

        // For backward compatibility with existing code
        public IAccountRepository AccountRepository => Accounts;
        public IServerInfoRepository ServerInfoRepository => ServerInfos;
        public IUserRepository UserRepository => Users;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
