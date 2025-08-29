using KnightDesk.Core.Domain.Entities;
using KnightDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KnightDesk.Infrastructure.Services
{
    public class DataSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeedingService> _logger;

        public DataSeedingService(ApplicationDbContext context, ILogger<DataSeedingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedServerInfosAsync()
        {
            try
            {
                // Check if data already exists
                if (await _context.ServerInfos.AnyAsync())
                {
                    _logger.LogInformation("ServerInfo data already exists. Skipping seeding.");
                    return;
                }

                var serverInfos = new List<ServerInfo>
                {
                    new ServerInfo { IndexServer = 0, Name = "Chiến Thần" },
                    new ServerInfo { IndexServer = 1, Name = "Rồng Lửa" },
                    new ServerInfo { IndexServer = 2, Name = "Global Server" },
                    new ServerInfo { IndexServer = 3, Name = "Phượng Hoàng" },
                    new ServerInfo { IndexServer = 4, Name = "Nhân Mã" },
                    new ServerInfo { IndexServer = 5, Name = "Kì Lân" },
                    new ServerInfo { IndexServer = 6, Name = "Thiên Hà" },
                    new ServerInfo { IndexServer = 7, Name = "Thách Đấu" }
                };

                await _context.ServerInfos.AddRangeAsync(serverInfos);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully seeded {serverInfos.Count} ServerInfo records.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding ServerInfo data.");
                throw;
            }
        }

        public async Task SeedAllAsync()
        {
            await SeedServerInfosAsync();
            // Add other seeding methods here
        }
    }
}
