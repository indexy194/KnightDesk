namespace KnightDesk.Core.Application.DTOs
{
    public class BaseAccountDTO
    {
        public string? Username { get; set; } // e.g., "a@.com, 012312312"
        public string? Password { get; set; }
        public int IndexCharacter { get; set; } // e.g., 1, 2, 3
        public int ServerInfoId { get; set; } // e.g., 1, 2, 3
        public bool IsFavorite { get; set; } = false;
        public string? CharacterName { get; set; } // e.g., "Knight, Wizard"
        public int UserId { get; set; } // e.g., 1, 2, 3

    }
    public class AccountDTO : BaseAccountDTO
    {
        public int Id { get; set; }
        public BaseServerInfoDTO? ServerInfo { get; set; } = null;
    }
    public class CreateAccountDTO : BaseAccountDTO
    {
    }

    public class UpdateAccountDTO : BaseAccountDTO
    {
        public int Id { get; set; }
    }
    public class UpdateCharacterDTO
    {
        public int Id { get; set; }
        public string CharacterName { get; set; } = string.Empty;
    }
}
