using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Application.Mappers
{
    public class ServerInfoProfile : Profile
    {
        public ServerInfoProfile()
        {
            CreateMap<ServerInfo, ServerInfoDTO>()
                .ForMember(dest => dest.Accounts, opt => opt.MapFrom(src => src.Accounts));
            CreateMap<ServerInfo, BaseServerInfoDTO>();

            CreateMap<CreateServerInfoDTO, ServerInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateServerInfoDTO, ServerInfo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
