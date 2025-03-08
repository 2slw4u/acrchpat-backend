using AutoMapper;
using CoreService.Models.Cache;
using CoreService.Models.Response.User;

namespace CoreService.Helpers.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<GetCurrentUserResponse, UserParametersCacheEntry>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.IsBanned,
                    opt => opt.MapFrom(src => src.IsBanned)
                )
                .ForMember(
                    dest => dest.Roles,
                    opt => opt.MapFrom(src => src.Roles)
                );
        }
    }
}
