using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Application.Mappers
{
    public class ServerInfoProfile : Profile
    {
        public ServerInfoProfile()
        {
            CreateMap<ServerInfo, ServerInfoDTO>();
        }
    }
}
