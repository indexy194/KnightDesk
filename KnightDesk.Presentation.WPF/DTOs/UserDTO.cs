namespace KnightDesk.Presentation.WPF.DTOs
{
    public class BaseUserDTO
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string IPAddress { get; set; } = null;
    }
    public class UserDTO : BaseUserDTO
    {
        public int Id { get; set; }
    }
    public class LoginResponseDTO : UserDTO
    {
        public bool IsSuccess { get; set; }
    }
}
