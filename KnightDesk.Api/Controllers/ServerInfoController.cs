using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnightDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerInfoController : ControllerBase
    {
        private readonly IServerInfoService _serverInfoService;
        private readonly ILogger<ServerInfoController> _logger;

        public ServerInfoController(IServerInfoService serverInfoService, ILogger<ServerInfoController> logger)
        {
            _serverInfoService = serverInfoService;
            _logger = logger;
        }

        /// <summary>
        /// Get all servers
        /// </summary>
        /// <returns>List of servers</returns>
        [HttpGet]
        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetAllServers()
        {
            try
            {
                var servers = await _serverInfoService.GetAllServersAsync();
                HttpContext.Response.StatusCode = servers.Code;
                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all servers");
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Get server by ID
        /// </summary>
        /// <param name="id">Server ID</param>
        /// <returns>Server details</returns>
        [HttpGet("{id}")]
        public async Task<GeneralResponseDTO<ServerInfoDTO?>> GetServer(int id)
        {
            try
            {
                var server = await _serverInfoService.GetServerByIdAsync(id);
                HttpContext.Response.StatusCode = server.Code;
                return server;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server {ServerId}", id);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Create new server
        /// </summary>
        /// <param name="serverDto">Server information to create</param>
        /// <returns>Created server information</returns>
        [HttpPost("add-server")]
        public async Task<GeneralResponseDTO<ServerInfoDTO>> AddServer([FromBody] CreateServerInfoDTO serverDto)
        {
            try
            {
                var server = await _serverInfoService.CreateServerAsync(serverDto);
                HttpContext.Response.StatusCode = server.Code;
                return server;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding server");
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Update server
        /// </summary>
        /// <param name="serverDto">Updated server information</param>
        /// <returns>Updated server information</returns>
        [HttpPut("update-server")]
        public async Task<GeneralResponseDTO<ServerInfoDTO>> UpdateServer([FromBody] UpdateServerInfoDTO serverDto)
        {
            try
            {
                var server = await _serverInfoService.UpdateServerAsync(serverDto);
                HttpContext.Response.StatusCode = server.Code;
                return server;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server");
                return new GeneralResponseDTO<ServerInfoDTO>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Delete server
        /// </summary>
        /// <param name="id">Server ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<GeneralResponseDTO<bool>> DeleteServer(int id)
        {
            try
            {
                var server = await _serverInfoService.DeleteServerAsync(id);
                HttpContext.Response.StatusCode = server.Code;
                return server;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server {ServerId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Search servers by name, index, or description
        /// </summary>
        /// <param name="searchText">Search query</param>
        /// <returns>List of matching servers</returns>
        [HttpGet("search")]
        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> SearchServers([FromQuery] string searchText = "")
        {
            try
            {
                var servers = await _serverInfoService.SearchServersAsync(searchText);
                HttpContext.Response.StatusCode = servers.Code;
                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching servers with term '{SearchTerm}'", searchText);
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Get server by index
        /// </summary>
        /// <param name="indexServer">Server index</param>
        /// <returns>Server details</returns>
        [HttpGet("index/{indexServer}")]
        public async Task<GeneralResponseDTO<ServerInfoDTO?>> GetServerByIndex(int indexServer)
        {
            try
            {
                var server = await _serverInfoService.GetServerByIndexAsync(indexServer);
                HttpContext.Response.StatusCode = server.Code;
                return server;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server by index {IndexServer}", indexServer);
                return new GeneralResponseDTO<ServerInfoDTO?>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }

        /// <summary>
        /// Get active servers only
        /// </summary>
        /// <returns>List of active servers</returns>
        [HttpGet("active")]
        public async Task<GeneralResponseDTO<IEnumerable<ServerInfoDTO>>> GetActiveServers()
        {
            try
            {
                var servers = await _serverInfoService.GetActiveServersAsync();
                HttpContext.Response.StatusCode = servers.Code;
                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active servers");
                return new GeneralResponseDTO<IEnumerable<ServerInfoDTO>>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    }
                };
            }
        }
    }
}
