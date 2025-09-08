namespace KnightDesk.Presentation.WPF.DTOs
{
    public class BaseAccountDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int IndexCharacter { get; set; }
        public int ServerInfoId { get; set; }
        public bool IsFavorite { get; set; } = false;
        public int UserId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
    }

    public class AccountDTO : BaseAccountDTO
    {
        public int Id { get; set; }
        public BaseServerInfoDTO ServerInfo { get; set; } = new BaseServerInfoDTO();
    }

    public class CreateAccountDTO : BaseAccountDTO
    {
    }

    public class UpdateAccountDTO : BaseAccountDTO
    {
        public int Id { get; set; }
    }
}
