using AutoMapper;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Enum;
using CoreService.Models.Http.Request.Transaction;

namespace CoreService.Helpers.Mappers
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile() {
            
            CreateMap<TransactionEntity, TransactionDTO>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => opt.Id)
                )
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(opt => opt.Amount)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(opt => opt.Type)
                )
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.MapFrom(opt => opt.PerformedAt)
                );

            CreateMap<TransactionEntity, DetailedTransactionDTO>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => opt.Id)
                )
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(opt => opt.Amount)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(opt => opt.Type)
                )
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.MapFrom(opt => opt.PerformedAt)
                );

            CreateMap<DepositMoneyToAccountRequest, TransactionEntity>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => Guid.NewGuid())
                )
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(opt => opt.Deposit.Amount)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(opt => TransactionType.Deposit)
                )
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.MapFrom(opt => DateTime.UtcNow)
                );

            CreateMap<WithdrawMoneyFromAccountRequest, TransactionEntity>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(opt => Guid.NewGuid())
                )
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(opt => opt.Withdrawal.Amount)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(opt => TransactionType.Withdrawal)
                )
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.MapFrom(opt => DateTime.UtcNow)
                );
            CreateMap<TransactionRequestDTO, TransactionEntity>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => Guid.NewGuid())
                )
                .ForMember(
                    dest => dest.Account,
                    opt => opt.MapFrom(src => src.AccountId)
                )
                .ForMember(
                    dest => dest.Amount,
                    opt => opt.MapFrom(src => src.Amount)
                )
                .ForMember(
                    dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type)
                )
                .ForMember(
                    dest => dest.PerformedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow)
                );
        }
    }
}
