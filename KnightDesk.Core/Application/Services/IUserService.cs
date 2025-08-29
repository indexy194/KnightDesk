using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;

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
}
