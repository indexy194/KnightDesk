namespace KnightDesk.Presentation.WPF.DTOs
{
    

    public class BaseUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
    }
    public class UserDTO : BaseUserDTO
    {
        public int Id { get; set; }
    }
    public class LoginRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
