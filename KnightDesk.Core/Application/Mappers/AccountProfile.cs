using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Application.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfo != null ? src.ServerInfo.Id : 0))
                .ForMember(dest => dest.ServerInfo, opt => opt.MapFrom(src => src.ServerInfo));

            CreateMap<CreateAccountDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Let database generate Id
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfoId))
                .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()); // Navigation property
                
            CreateMap<UpdateAccountDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Preserve existing Id
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfoId))
                .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Preserve audit fields
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
