using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIPAddressAsync(string ipAddress);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByIPAddressAsync(string ipAddress);
        Task<User?> ValidateUserCredentialsAsync(string username, string password);
    }
}
