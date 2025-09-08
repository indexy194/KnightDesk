using AutoMapper;
using KnightDesk.Core.Application.DTOs;
using KnightDesk.Core.Domain.Entities;

namespace KnightDesk.Core.Application.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            //Entity to DTO mappings
            CreateMap<Account, AccountDTO>();
            CreateMap<Account, BaseAccountDTO>();

            //DTO to Entity mappings
            CreateMap<CreateAccountDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfoId))
                .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Preserve audit fields
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            CreateMap<UpdateAccountDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Preserve existing Id
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfoId))
                .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Preserve audit fields
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Base DTO mappings
            CreateMap<BaseAccountDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ServerInfoId, opt => opt.MapFrom(src => src.ServerInfoId))
                .ForMember(dest => dest.ServerInfo, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Preserve audit fields
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
