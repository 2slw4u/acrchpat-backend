using AutoMapper;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Enum;

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
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                )
                .ForMember(
                    dest => dest.Currency,
                    opt => opt.MapFrom(opt => opt.Currency)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => AccountType.UserCreditAccount)
                )
                .ForMember(
                    dest => dest.Number,
                    opt => opt.Ignore()
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
                )
                .ForMember(
                    dest => dest.Currency,
                    opt => opt.MapFrom(opt => opt.Currency)
                )
                .ForMember(
                    dest => dest.Number,
                    opt => opt.MapFrom(opt => opt.Number)
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
                )
                .ForMember(
                    dest => dest.Currency,
                    opt => opt.MapFrom(opt => opt.Currency)
                )
                .ForMember(
                    dest => dest.Number,
                    opt => opt.MapFrom(opt => opt.Number)
                );
        }
    }
}
