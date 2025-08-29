namespace KnightDesk.Core.Domain.Entities
{
    public class User : CommonEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? IPAddress { get; set; }
        public virtual ICollection<Account>? Accounts { get; set; } = null;
    }
}
