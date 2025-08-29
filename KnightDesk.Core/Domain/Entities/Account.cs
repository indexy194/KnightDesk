namespace KnightDesk.Core.Domain.Entities
{
    public class Account : CommonEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; } // e.g., "a@.com, 012312312"
        public string? CharacterName { get; set; } // e.g., "Knight, Wizard"
        public string? Password { get; set; }
        public int IndexCharacter { get; set; } // e.g., 1, 2, 3
        public bool IsFavorite { get; set; } = false;
        public int ServerInfoId { get; set; } // e.g., 1, 2, 3
        public virtual ServerInfo? ServerInfo { get; set; }
        public int UserId { get; set; } // e.g., 1, 2, 3
        public virtual User? User { get; set; }
    }
}
