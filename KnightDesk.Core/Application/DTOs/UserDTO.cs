namespace KnightDesk.Core.Application.DTOs
{
    public class BaseUserDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? IPAddress { get; set; }
    }
    public class UserDTO : BaseUserDTO
    {
        public int Id { get; set; }
    }

    public class CreateUserDTO : BaseUserDTO
    {

    }
    public class UpdateUserDTO : BaseUserDTO
    {
        public int Id { get; set; }
    }
    public class RegisterUserDTO : BaseUserDTO
    {
    }
    public class LoginUserDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
