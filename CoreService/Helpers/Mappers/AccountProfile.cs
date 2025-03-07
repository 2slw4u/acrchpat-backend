using AutoMapper;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;

namespace CoreService.Helpers.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile() 
        {
            CreateMap<AccountCreateModel, AccountEntity>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid())
                )
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.Name}")
                )
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Owner)
                )
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                );

            CreateMap<AccountEntity, AccountDTO>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => opt.Id)
                )
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(opt => opt.Name)
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(opt => opt.Status)
                )
                .ForMember(
                    dest => dest.Balance,
                    opt => opt.MapFrom(opt => opt.Balance)
                );

            CreateMap<AccountEntity, DetailedAccountDTO>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => opt.Id)
                )
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(opt => opt.Name)
                )
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(opt => opt.UserId)
                )
                .ForMember(
                    dest => dest.CreatedDate,
                    opt => opt.MapFrom(opt => opt.CreatedAt)
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(opt => opt.Status)
                )
                .ForMember(
                    dest => dest.Balance,
                    opt => opt.MapFrom(opt => opt.Balance)
                );
        }
    }
}
