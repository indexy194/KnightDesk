using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Domain.Interfaces
{
    public interface IServerInfoRepository : IRepository<ServerInfo>
    {
        Task<ServerInfo?> GetByServerNoAsync(int serverNo);
        Task<IEnumerable<ServerInfo>> GetActiveServersAsync();
        Task<bool> IsServerNoExistsAsync(int serverNo);
    }
}
