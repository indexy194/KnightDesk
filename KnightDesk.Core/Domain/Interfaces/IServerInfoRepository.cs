using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Domain.Interfaces
{
    public interface IServerInfoRepository : IRepository<ServerInfo>
    {
        Task<ServerInfo?> GetByIndexServerAsync(int indexServer);
        Task<ServerInfo> GetServerInfoByIndexServerAsync(int indexServer);
        Task<IEnumerable<ServerInfo>> GetActiveServersAsync();
        Task<bool> IsIndexServerExistsAsync(int indexServer);
    }
}
