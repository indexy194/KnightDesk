using System;
using System.Collections.Generic;
using KnightDesk.Presentation.WPF.Models;

namespace KnightDesk.Presentation.WPF.Services
{
    public  interface IServerInfoService
    {
        void GetAllServersAsync(Action<List<ServerInfo>> callback);
    }
}
