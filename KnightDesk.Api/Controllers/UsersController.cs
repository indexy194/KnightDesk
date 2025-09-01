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
        public async Task<GeneralResponseDTO<IEnumerable<UserDTO>>> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new GeneralResponseDTO<IEnumerable<UserDTO>>
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
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details with response metadata</returns>
        [HttpGet("{id}")]
        public async Task<GeneralResponseDTO<UserDTO>> GetUser(int id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);
                HttpContext.Response.StatusCode = result.Code;
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", id);
                return new GeneralResponseDTO<UserDTO>
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
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User details with response metadata ( admin func )</returns>
        [HttpGet("username/{username}")]
        public async Task<GeneralResponseDTO<UserDTO>> GetUserByUsername(string username)
        {
            try
            {
                var result = await _userService.GetUserByUsernameAsync(username);
                HttpContext.Response.StatusCode = result.Code;
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username {Username}", username);
                return new GeneralResponseDTO<UserDTO>
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
        /// Get user by IP address
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <returns>User details with response metadata</returns>
        [HttpGet("ip/{ipAddress}")]
        public async Task<GeneralResponseDTO<UserDTO>> GetUserByIPAddress(string ipAddress)
        {
            try
            {
                var result = await _userService.GetUserByIPAddressAsync(ipAddress);
                HttpContext.Response.StatusCode = result.Code;
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by IP address {IPAddress}", ipAddress);
                return new GeneralResponseDTO<UserDTO>
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
        /// Create a new user
        /// </summary>
        /// <param name="userDto">User creation data</param>
        /// <returns>Created user with response metadata</returns>
        [HttpPost]
        public async Task<GeneralResponseDTO<UserDTO>> CreateUser([FromBody] CreateUserDTO userDto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(userDto);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with username {Username}", userDto?.Username);
                return new GeneralResponseDTO<UserDTO>
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
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userDto">User update data</param>
        /// <returns>Updated user with response metadata</returns>
        [HttpPut("{id}")]
        public async Task<GeneralResponseDTO<UserDTO>> UpdateUser([FromBody] UpdateUserDTO userDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(userDto);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", userDto?.Id);
                return new GeneralResponseDTO<UserDTO>
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
        /// Delete a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Deletion result with response metadata</returns>
        [HttpDelete("{id}")]
        public async Task<GeneralResponseDTO<bool>> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }
        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>User data if login successful with response metadata</returns>
        [HttpPost("login")]
        public async Task<GeneralResponseDTO<UserDTO>> Login([FromBody] LoginUserDTO loginRequest)
        {
            try
            {
                var result = await _userService.LoginAsync(loginRequest);
                HttpContext.Response.StatusCode = result.Code;
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", loginRequest?.Username);
                return new GeneralResponseDTO<UserDTO>
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
        /// User registration
        /// </summary>
        /// <param name="registerRequest">Registration data</param>
        /// <returns>Created user with response metadata</returns>
        [HttpPost("register")]
        public async Task<GeneralResponseDTO<UserDTO>> Register([FromBody] RegisterUserDTO registerRequest)
        {
            try
            {
                var result = await _userService.RegisterAsync(registerRequest);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}", registerRequest?.Username);
                return new GeneralResponseDTO<UserDTO>
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
        /// Validate user credentials
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Validation result with response metadata</returns>
        [HttpPost("validate-credentials")]
        public async Task<GeneralResponseDTO<bool>> ValidateCredentials([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                var result = await _userService.ValidateCredentialsAsync(username, password);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for user {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Availability result with response metadata</returns>
        [HttpGet("check-username/{username}")]
        public async Task<GeneralResponseDTO<bool>> CheckUsernameAvailability(string username)
        {
            try
            {
                var result = await _userService.IsUsernameAvailableAsync(username);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username availability for {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }

        /// <summary>
        /// Check if IP address is available
        /// </summary>
        /// <param name="ipAddress">IP address to check</param>
        /// <returns>Availability result with response metadata</returns>
        [HttpGet("check-ip/{ipAddress}")]
        public async Task<GeneralResponseDTO<bool>> CheckIPAddressAvailability(string ipAddress)
        {
            try
            {
                var result = await _userService.IsIPAddressAvailableAsync(ipAddress);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking IP address availability for {IPAddress}", ipAddress);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }

        /// <summary>
        /// Check if user exists by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Existence result with response metadata</returns>
        [HttpGet("exists/{id}")]
        public async Task<GeneralResponseDTO<bool>> UserExists(int id)
        {
            try
            {
                var result = await _userService.UserExistsAsync(id);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence for ID {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }

        /// <summary>
        /// Check if user exists by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Existence result with response metadata</returns>
        [HttpGet("exists/username/{username}")]
        public async Task<GeneralResponseDTO<bool>> UserExistsByUsername(string username)
        {
            try
            {
                var result = await _userService.UserExistsByUsernameAsync(username);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence for username {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        } 

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="passwordChangeRequest">Password change data</param>
        /// <returns>Change result with response metadata</returns>
        [HttpPut("{id}/change-password")]
        public async Task<GeneralResponseDTO<bool>> ChangePassword(int id, [FromBody] ChangePasswordRequest passwordChangeRequest)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(id, passwordChangeRequest.CurrentPassword, passwordChangeRequest.NewPassword);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="resetRequest">Password reset data</param>
        /// <returns>Reset result with response metadata</returns>
        [HttpPost("reset-password")]
        public async Task<GeneralResponseDTO<bool>> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(resetRequest.Username, resetRequest.NewPassword);
                HttpContext.Response.StatusCode = result.Code;
                return result;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for username {Username}", resetRequest?.Username);
                return new GeneralResponseDTO<bool>
                {
                    Code = 500,
                    Message = "Internal server error",
                    Errors = new List<ResponseError>
                    {
                        new ResponseError { Code = 500, Message = "An error occurred while processing your request." }
                    },
                    Data = false
                };
            }
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