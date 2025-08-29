using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnightDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users with response metadata</returns>
        [HttpGet]
        public async Task<ActionResult<GeneralResponseDTO<IEnumerable<UserDTO>>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return result;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details with response metadata</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO?>>> GetUser(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            HttpContext.Response.StatusCode = result.Code;
            return result;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User details with response metadata ( admin func )</returns>
        [HttpGet("username/{username}")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO?>>> GetUserByUsername(string username)
        {
            var result = await _userService.GetUserByUsernameAsync(username);
            HttpContext.Response.StatusCode = result.Code;
            return result;
        }

        /// <summary>
        /// Get user by IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>User details with response metadata</returns>
        [HttpGet("ip/{ipAddress}")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO?>>> GetUserByIPAddress(string ipAddress)
        {
            var result = await _userService.GetUserByIPAddressAsync(ipAddress);
            HttpContext.Response.StatusCode = result.Code;
            return result;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userDto">User creation data</param>
        /// <returns>Created user with response metadata</returns>
        [HttpPost]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO>>> CreateUser([FromBody] CreateUserDTO userDto)
        {
            var result = await _userService.CreateUserAsync(userDto);
            HttpContext.Response.StatusCode = result.Code;
            return result;
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userDto">User update data</param>
        /// <returns>Updated user with response metadata</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO>>> UpdateUser([FromBody] UpdateUserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest(new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.BadRequest,
                    Message = "User data is required"
                });
            }
            var result = await _userService.UpdateUserAsync(userDto);
            HttpContext.Response.StatusCode = result.Code;
            return result;
        }

        /// <summary>
        /// Delete a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Deletion result with response metadata</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>User data if login successful with response metadata</returns>
        [HttpPost("login")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO?>>> Login([FromBody] LoginUserDTO loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest(new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.BadRequest,
                    Message = "Login data is required"
                });
            }

            var result = await _userService.LoginAsync(loginRequest);
            return result;
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerRequest">Registration data</param>
        /// <returns>Created user with response metadata</returns>
        [HttpPost("register")]
        public async Task<ActionResult<GeneralResponseDTO<UserDTO>>> Register([FromBody] RegisterUserDTO registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest(new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.BadRequest,
                    Message = "Registration data is required"
                });
            }

            var result = await _userService.RegisterAsync(registerRequest);
            return result;
        }

        /// <summary>
        /// Validate user credentials
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Validation result with response metadata</returns>
        [HttpPost("validate-credentials")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> ValidateCredentials([FromQuery] string username, [FromQuery] string password)
        {
            var result = await _userService.ValidateCredentialsAsync(username, password);
            return result;
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Availability result with response metadata</returns>
        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> CheckUsernameAvailability(string username)
        {
            var result = await _userService.IsUsernameAvailableAsync(username);
            return result;
        }

        /// <summary>
        /// Check if IP address is available
        /// </summary>
        /// <param name="ipAddress">IP address to check</param>
        /// <returns>Availability result with response metadata</returns>
        [HttpGet("check-ip/{ipAddress}")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> CheckIPAddressAvailability(string ipAddress)
        {
            var result = await _userService.IsIPAddressAvailableAsync(ipAddress);
            return result;
        }

        /// <summary>
        /// Check if user exists by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Existence result with response metadata</returns>
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> UserExists(int id)
        {
            var result = await _userService.UserExistsAsync(id);
            return result;
        }

        /// <summary>
        /// Check if user exists by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Existence result with response metadata</returns>
        [HttpGet("exists/username/{username}")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> UserExistsByUsername(string username)
        {
            var result = await _userService.UserExistsByUsernameAsync(username);
            return result;
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="passwordChangeRequest">Password change data</param>
        /// <returns>Change result with response metadata</returns>
        [HttpPut("{id}/change-password")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> ChangePassword(int id, [FromBody] ChangePasswordRequest passwordChangeRequest)
        {
            if (passwordChangeRequest == null)
            {
                return BadRequest(new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.BadRequest,
                    Message = "Password change data is required",
                    Data = false
                });
            }

            var result = await _userService.ChangePasswordAsync(id, passwordChangeRequest.CurrentPassword, passwordChangeRequest.NewPassword);
            return result;
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="resetRequest">Password reset data</param>
        /// <returns>Reset result with response metadata</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult<GeneralResponseDTO<bool>>> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            if (resetRequest == null)
            {
                return BadRequest(new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.BadRequest,
                    Message = "Password reset data is required",
                    Data = false
                });
            }

            var result = await _userService.ResetPasswordAsync(resetRequest.Username, resetRequest.NewPassword);
            return result;
        }
    }

    // Helper DTOs for controller operations
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}