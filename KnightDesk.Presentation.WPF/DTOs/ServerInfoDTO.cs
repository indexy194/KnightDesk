using System.Collections.Generic;

namespace KnightDesk.Presentation.WPF.DTOs
{
    public class BaseServerInfoDTO
    {
        public int IndexServer { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ServerInfoDTO : BaseServerInfoDTO
    {
        public int Id { get; set; }
        public List<BaseAccountDTO> Accounts { get; set; } = new List<BaseAccountDTO>();
    }

    public class CreateServerInfoDTO : BaseServerInfoDTO
    {
    }

    public class UpdateServerInfoDTO : BaseServerInfoDTO
    {
        public int Id { get; set; }
    }

}
