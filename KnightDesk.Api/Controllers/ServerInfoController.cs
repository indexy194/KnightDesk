using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KnightDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerInfoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServerInfoController> _logger;

        public ServerInfoController(IUnitOfWork unitOfWork, ILogger<ServerInfoController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all server information
        /// </summary>
        /// <returns>List of server information</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerInfo>>> GetAllServerInfo()
        {
            try
            {
                var serverInfos = await _unitOfWork.ServerInfos.GetAllAsync();
                return Ok(serverInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all server info");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get server information by ID
        /// </summary>
        /// <param name="id">Server info ID</param>
        /// <returns>Server information details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerInfo>> GetServerInfo(int id)
        {
            try
            {
                var serverInfo = await _unitOfWork.ServerInfos.GetByIdAsync(id);
                if (serverInfo == null)
                {
                    return NotFound($"Server info with ID {id} not found");
                }
                return Ok(serverInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting server info {ServerInfoId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create new server information
        /// </summary>
        /// <param name="serverInfo">Server information to create</param>
        /// <returns>Created server information</returns>
        [HttpPost]
        public async Task<ActionResult<ServerInfo>> CreateServerInfo([FromBody] ServerInfo serverInfo)
        {
            try
            {
                if (serverInfo == null)
                {
                    return BadRequest("Server info cannot be null");
                }

                await _unitOfWork.ServerInfos.AddAsync(serverInfo);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetServerInfo), new { id = serverInfo.Id }, serverInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating server info");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update server information
        /// </summary>
        /// <param name="id">Server info ID</param>
        /// <param name="serverInfo">Updated server information</param>
        /// <returns>Updated server information</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ServerInfo>> UpdateServerInfo(int id, [FromBody] ServerInfo serverInfo)
        {
            try
            {
                if (serverInfo == null || serverInfo.Id != id)
                {
                    return BadRequest("Invalid server info data");
                }

                var existingServerInfo = await _unitOfWork.ServerInfos.GetByIdAsync(id);
                if (existingServerInfo == null)
                {
                    return NotFound($"Server info with ID {id} not found");
                }

                await _unitOfWork.ServerInfos.UpdateAsync(serverInfo);
                await _unitOfWork.SaveChangesAsync();

                return Ok(serverInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server info {ServerInfoId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete server information
        /// </summary>
        /// <param name="id">Server info ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteServerInfo(int id)
        {
            try
            {
                var serverInfo = await _unitOfWork.ServerInfos.GetByIdAsync(id);
                if (serverInfo == null)
                {
                    return NotFound($"Server info with ID {id} not found");
                }

                await _unitOfWork.ServerInfos.DeleteAsync(serverInfo);
                await _unitOfWork.SaveChangesAsync();

                return Ok($"Server info with ID {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server info {ServerInfoId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
