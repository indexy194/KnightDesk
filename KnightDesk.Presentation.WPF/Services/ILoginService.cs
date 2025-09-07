using KnightDesk.Presentation.WPF.DTOs;
using System;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface ILoginService
    {
        void LoginAsync(string username, string password, Action<GeneralResponseDTO<LoginResponseDTO>> callback);
    }
}
