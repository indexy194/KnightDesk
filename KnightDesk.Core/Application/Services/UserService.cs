using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;
using KnightDesk.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KnightDesk.Core.Application.Services
{
    public interface IUserService
    {
        // Basic CRUD operations
        Task<GeneralResponseDTO<IEnumerable<UserDTO>>> GetAllUsersAsync();
        Task<GeneralResponseDTO<UserDTO?>> GetUserByIdAsync(int id);
        Task<GeneralResponseDTO<UserDTO?>> GetUserByUsernameAsync(string username);
        Task<GeneralResponseDTO<UserDTO?>> GetUserByIPAddressAsync(string ipAddress);
        Task<GeneralResponseDTO<UserDTO>> CreateUserAsync(CreateUserDTO user);
        Task<GeneralResponseDTO<UserDTO>> UpdateUserAsync(UpdateUserDTO user);
        Task<GeneralResponseDTO<bool>> DeleteUserAsync(int id);

        // Authentication operations
        Task<GeneralResponseDTO<UserDTO?>> LoginAsync(LoginUserDTO loginRequest);
        Task<GeneralResponseDTO<UserDTO>> RegisterAsync(RegisterUserDTO registerRequest);
        Task<GeneralResponseDTO<bool>> ValidateCredentialsAsync(string username, string password);

        // Validation operations
        Task<GeneralResponseDTO<bool>> IsUsernameAvailableAsync(string username);
        Task<GeneralResponseDTO<bool>> IsIPAddressAvailableAsync(string ipAddress);
        Task<GeneralResponseDTO<bool>> UserExistsAsync(int id);
        Task<GeneralResponseDTO<bool>> UserExistsByUsernameAsync(string username);

        // Password operations
        Task<GeneralResponseDTO<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<GeneralResponseDTO<bool>> ResetPasswordAsync(string username, string newPassword);
    }
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Basic CRUD Operations

        public async Task<GeneralResponseDTO<IEnumerable<UserDTO>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                return new GeneralResponseDTO<IEnumerable<UserDTO>>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Users retrieved successfully",
                    Data = _mapper.Map<IEnumerable<UserDTO>>(users)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new GeneralResponseDTO<IEnumerable<UserDTO>>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving all users"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO?>> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user ID",
                        Data = null
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"User with ID {id} not found",
                        Data = null
                    };
                }

                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "User retrieved successfully",
                    Data = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", id);
                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the user"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO?>> GetUserByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username cannot be empty",
                        Data = null
                    };
                }

                var user = await _unitOfWork.Users.GetByUsernameAsync(username);
                if (user == null)
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"User with username '{username}' not found",
                        Data = null
                    };
                }

                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "User retrieved successfully",
                    Data = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username {Username}", username);
                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the user"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO?>> GetUserByIPAddressAsync(string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "IP address cannot be empty",
                        Data = null
                    };
                }

                var user = await _unitOfWork.Users.GetByIPAddressAsync(ipAddress);
                if (user == null)
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"User with IP address '{ipAddress}' not found",
                        Data = null
                    };
                }

                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "User retrieved successfully",
                    Data = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by IP address {IPAddress}", ipAddress);
                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while retrieving the user"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO>> CreateUserAsync(CreateUserDTO userDto)
        {
            try
            {
                // Validate user data
                var validationResult = ValidateUser(userDto);
                if (!validationResult)
                {
                    return new GeneralResponseDTO<UserDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user data"
                    };
                }

                // Check if username already exists
                if (!string.IsNullOrWhiteSpace(userDto.Username))
                {
                    var usernameExistsResult = await IsUsernameAvailableAsync(userDto.Username);
                    if (!usernameExistsResult.Data)
                    {
                        return new GeneralResponseDTO<UserDTO>
                        {
                            Code = (int)RESPONSE_CODE.BadRequest,
                            Message = $"Username '{userDto.Username}' is already taken"
                        };
                    }
                }

                var user = _mapper.Map<User>(userDto);
                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Username}", userDto.Username);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.Created,
                    Message = "User created successfully",
                    Data = _mapper.Map<UserDTO>(createdUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Username}", userDto.Username);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while creating the user"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO>> UpdateUserAsync(UpdateUserDTO userDto)
        {
            try
            {
                // Validate user data
                var validationResult = ValidateUser(userDto);
                if (!validationResult)
                {
                    return new GeneralResponseDTO<UserDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user data"
                    };
                }

                // Check if user exists
                var existingUser = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                {
                    return new GeneralResponseDTO<UserDTO>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"User with ID {userDto.Id} not found"
                    };
                }

                // Check if username is being changed and if it's available
                if (!string.IsNullOrWhiteSpace(userDto.Username) && 
                    userDto.Username != existingUser.Username)
                {
                    var usernameExistsResult = await IsUsernameAvailableAsync(userDto.Username);
                    if (!usernameExistsResult.Data)
                    {
                        return new GeneralResponseDTO<UserDTO>
                        {
                            Code = (int)RESPONSE_CODE.BadRequest,
                            Message = $"Username '{userDto.Username}' is already taken"
                        };
                    }
                }

                var user = _mapper.Map<User>(userDto);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User updated successfully: {UserId}", userDto.Id);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "User updated successfully",
                    Data = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userDto.Id);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while updating the user"
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user ID",
                        Data = false
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = $"User with ID {id} not found",
                        Data = false
                    };
                }

                // Soft delete
                user.IsDeleted = true;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User soft deleted successfully: {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "User deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while deleting the user",
                    Data = false
                };
            }
        }

        #endregion

        #region Authentication Operations

        public async Task<GeneralResponseDTO<UserDTO?>> LoginAsync(LoginUserDTO loginRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username and password are required",
                        Data = null
                    };
                }

                var user = await _unitOfWork.Users.ValidateUserCredentialsAsync(loginRequest.Username, loginRequest.Password);
                
                if (user != null)
                {
                    _logger.LogInformation("User {Username} logged in successfully", loginRequest.Username);
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.OK,
                        Message = "Login successful",
                        Data = _mapper.Map<UserDTO>(user)
                    };
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for username {Username}", loginRequest.Username);
                    return new GeneralResponseDTO<UserDTO?>
                    {
                        Code = (int)RESPONSE_CODE.Unauthorized,
                        Message = "Invalid username or password",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username {Username}", loginRequest.Username);
                return new GeneralResponseDTO<UserDTO?>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<GeneralResponseDTO<UserDTO>> RegisterAsync(RegisterUserDTO registerRequest)
        {
            try
            {
                // Validate registration data
                var validationResult = ValidateUser(registerRequest);
                if (!validationResult)
                {
                    return new GeneralResponseDTO<UserDTO>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid registration data"
                    };
                }

                // Check if username is available
                if (!string.IsNullOrWhiteSpace(registerRequest.Username))
                {
                    var usernameExistsResult = await IsUsernameAvailableAsync(registerRequest.Username);
                    if (!usernameExistsResult.Data)
                    {
                        return new GeneralResponseDTO<UserDTO>
                        {
                            Code = (int)RESPONSE_CODE.BadRequest,
                            Message = $"Username '{registerRequest.Username}' is already taken"
                        };
                    }
                }

                var user = _mapper.Map<User>(registerRequest);
                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {Username}", registerRequest.Username);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.Created,
                    Message = "User registered successfully",
                    Data = _mapper.Map<UserDTO>(createdUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", registerRequest.Username);
                return new GeneralResponseDTO<UserDTO>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username and password are required",
                        Data = false
                    };
                }

                var user = await _unitOfWork.Users.ValidateUserCredentialsAsync(username, password);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = user != null ? "Credentials are valid" : "Invalid credentials",
                    Data = user != null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for username {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while validating credentials",
                    Data = false
                };
            }
        }

        #endregion

        #region Validation Operations

        public async Task<GeneralResponseDTO<bool>> IsUsernameAvailableAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username cannot be empty",
                        Data = false
                    };
                }

                var exists = await _unitOfWork.Users.ExistsByUsernameAsync(username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = exists ? "Username is not available" : "Username is available",
                    Data = !exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username availability {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking username availability",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> IsIPAddressAvailableAsync(string ipAddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.OK,
                        Message = "IP address is available (empty is allowed)",
                        Data = true
                    };
                }

                var exists = await _unitOfWork.Users.ExistsByIPAddressAsync(ipAddress);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = exists ? "IP address is not available" : "IP address is available",
                    Data = !exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking IP address availability {IPAddress}", ipAddress);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking IP address availability",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> UserExistsAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Invalid user ID",
                        Data = false
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = user != null ? "User exists" : "User does not exist",
                    Data = user != null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence {UserId}", id);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking user existence",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> UserExistsByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username cannot be empty",
                        Data = false
                    };
                }

                var exists = await _unitOfWork.Users.ExistsByUsernameAsync(username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = exists ? "User exists" : "User does not exist",
                    Data = exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence by username {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while checking user existence",
                    Data = false
                };
            }
        }

        #endregion

        #region Password Operations

        public async Task<GeneralResponseDTO<bool>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                if (userId <= 0 || string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "User ID, current password, and new password are required",
                        Data = false
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = "User not found",
                        Data = false
                    };
                }

                if (user.Password != currentPassword)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Current password is incorrect",
                        Data = false
                    };
                }

                user.Password = newPassword; // In production, hash this password
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Password changed successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while changing password",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponseDTO<bool>> ResetPasswordAsync(string username, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.BadRequest,
                        Message = "Username and new password are required",
                        Data = false
                    };
                }

                var user = await _unitOfWork.Users.GetByUsernameAsync(username);
                if (user == null)
                {
                    return new GeneralResponseDTO<bool>
                    {
                        Code = (int)RESPONSE_CODE.NotFound,
                        Message = "User not found",
                        Data = false
                    };
                }

                user.Password = newPassword; // In production, hash this password
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully for user {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.OK,
                    Message = "Password reset successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for username {Username}", username);
                return new GeneralResponseDTO<bool>
                {
                    Code = (int)RESPONSE_CODE.InternalServerError,
                    Message = "An error occurred while resetting password",
                    Data = false
                };
            }
        }

        #endregion

        #region Private Helper Methods

        private bool ValidateUser(BaseUserDTO user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(user.Username))
                return false;

            if (string.IsNullOrWhiteSpace(user.Password))
                return false;

            return true;
        }

        #endregion
    }
}