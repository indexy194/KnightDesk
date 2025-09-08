using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KnightDesk.Core.Application.Services
{
    public interface IServerInfoService
    {
        Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetAllServersAsync();
        Task<GeneralResponseDTO<ServerInfoDTO?>> GetServerByIdAsync(int id);
        Task<GeneralResponseDTO<ServerInfoDTO?>> GetServerByIndexAsync(int indexServer);
        Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetActiveServersAsync();
        Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> SearchServersAsync(string searchTerm);
        Task<GeneralResponseDTO<ServerInfoDTO>> CreateServerAsync(CreateServerInfoDTO server);
        Task<GeneralResponseDTO<ServerInfoDTO>> UpdateServerAsync(UpdateServerInfoDTO server);
        Task<GeneralResponseDTO<bool>> DeleteServerAsync(int id);
        Task<GeneralResponseDTO<bool>> IsIndexServerExistsAsync(int indexServer);
    }
    public class ServerInfoService : IServerInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ServerInfoService> _logger;

        public ServerInfoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ServerInfoService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetAllServersAsync()
        {
            try
            {
                var servers = await _unitOfWork.ServerInfos.GetAllAsync();
                var serverDTOs = _mapper.Map<IEnumerable<ServerInfoDTO>>(servers);

                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Servers retrieved successfully",
                    Data = serverDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all servers");
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving servers",
                    Data = new List<ServerInfoDTO>()
                };
            }
        }

        public async Task<GeneralResponseDTO<ServerInfoDTO?>> GetServerByIdAsync(int id)
        {
            try
            {
                var server = await _unitOfWork.ServerInfos.GetByIdAsync(id);
                if (server == null)
                {
                    return new GeneralResponseDTO<ServerInfoDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Server with ID {id} not found",
                        Data = null
                    };
                }

                var serverDTO = _mapper.Map<ServerInfoDTO>(server);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Server retrieved successfully",
                    Data = serverDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server with ID {ServerId}", id);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the server",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponseDTO<ServerInfoDTO?>> GetServerByIndexAsync(int indexServer)
        {
            try
            {
                var server = await _unitOfWork.ServerInfos.GetByIndexServerAsync(indexServer);
                if (server == null)
                {
                    return new GeneralResponseDTO<ServerInfoDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Server with index {indexServer} not found",
                        Data = null
                    };
                }

                var serverDTO = _mapper.Map<ServerInfoDTO>(server);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Server retrieved successfully",
                    Data = serverDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server with index {IndexServer}", indexServer);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the server",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetActiveServersAsync()
        {
            try
            {
                var servers = await _unitOfWork.ServerInfos.GetActiveServersAsync();
                var serverDTOs = _mapper.Map<IEnumerable<ServerInfoDTO>>(servers);

                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Active servers retrieved successfully",
                    Data = serverDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active servers");
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving active servers",
                    Data = new List<ServerInfoDTO>()
                };
            }
        }

        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> SearchServersAsync(string searchTerm)
        {
            try
            {
                var servers = await _unitOfWork.ServerInfos.GetAllAsync();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    var allServerDTOs = _mapper.Map<IEnumerable<ServerInfoDTO>>(servers);
                    return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                    {
                        Code = (int)RESPONSE_CODE.OK,
                        Message = "All servers retrieved successfully",
                        Data = allServerDTOs
                    };
                }

                var filteredServers = servers.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) && s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    s.IndexServer.ToString().Contains(searchTerm))
                    .ToList();

                var serverDTOs = _mapper.Map<IEnumerable<ServerInfoDTO>>(filteredServers);
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = $"Found {filteredServers.Count} servers matching '{searchTerm}'",
                    Data = serverDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching servers with term '{SearchTerm}'", searchTerm);
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while searching servers",
                    Data = new List<ServerInfoDTO>()
                };
            }
        }

        public async Task<GeneralResponseDTO<ServerInfoDTO>> CreateServerAsync(CreateServerInfoDTO serverDto)
        {
            try
            {
                // Check if IndexServer already exists
                var existsResult = await IsIndexServerExistsAsync(serverDto.IndexServer);
                if (existsResult.Data)
                {
                    return new GeneralResponseDTO<ServerInfoDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = $"Server with index {serverDto.IndexServer} already exists"
                    };
                }

                var server = _mapper.Map<ServerInfo>(serverDto);

                await _unitOfWork.ServerInfos.AddAsync(server);
                await _unitOfWork.SaveChangesAsync();

                var serverDTO = _mapper.Map<ServerInfoDTO>(server);
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = (int)RESPONSE_CODE.Created,
                    Message = "Server created successfully",
                    Data = serverDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating server");
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while creating the server"
                };
            }
        }

        public async Task<GeneralResponseDTO<ServerInfoDTO>> UpdateServerAsync(UpdateServerInfoDTO serverDto)
        {
            try
            {
                var existingServer = await _unitOfWork.ServerInfos.GetByIdAsync(serverDto.Id);
                if (existingServer == null)
                {
                    return new GeneralResponseDTO<ServerInfoDTO>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Server with ID {serverDto.Id} not found"
                    };
                }

                // Check if IndexServer already exists (excluding current server)
                if (existingServer.IndexServer != serverDto.IndexServer)
                {
                    var existsResult = await IsIndexServerExistsAsync(serverDto.IndexServer);
                    if (existsResult.Data)
                    {
                        return new GeneralResponseDTO<ServerInfoDTO>
                        {
                            Code = (int)RESPONSE_CODE.BadRequest,
                            Message = $"Server with index {serverDto.IndexServer} already exists"
                        };
                    }
                }

                // Update properties
                existingServer.IndexServer = serverDto.IndexServer;
                existingServer.Name = serverDto.Name;

                await _unitOfWork.ServerInfos.UpdateAsync(existingServer);
                await _unitOfWork.SaveChangesAsync();

                var serverDTO = _mapper.Map<ServerInfoDTO>(existingServer);
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Server updated successfully",
                    Data = serverDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server with ID {ServerId}", serverDto.Id);
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while updating the server"
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> DeleteServerAsync(int id)
        {
            try
            {
                var server = await _unitOfWork.ServerInfos.GetByIdAsync(id);
                if (server == null)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"Server with ID {id} not found",
                        Data = false
                    };
                }

                // Check if server has accounts
                if (server.Accounts != null && server.Accounts.Any())
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Cannot delete server with existing accounts",
                        Data = false
                    };
                }

                // Soft delete
                server.IsDeleted = true;

                await _unitOfWork.ServerInfos.UpdateAsync(server);
                await _unitOfWork.SaveChangesAsync();

                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Server deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server with ID {ServerId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while deleting the server",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> IsIndexServerExistsAsync(int indexServer)
        {
            try
            {
                var exists = await _unitOfWork.ServerInfos.IsIndexServerExistsAsync(indexServer);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = exists ? "Index server exists" : "Index server does not exist",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if index server {IndexServer} exists", indexServer);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking index server existence",
                    Data = false
                };
            }
        }
    }
}
