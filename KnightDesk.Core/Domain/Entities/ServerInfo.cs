using System.ComponentModel.DataAnnotations;

namespace KnightDesk.Core.Domain.Entities
{
    public class ServerInfo : CommonEntity
    {
        public int Id { get; set; }
        [Required]
        public int IndexServer { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Account>? Accounts { get; set; } = null;
    }
}
