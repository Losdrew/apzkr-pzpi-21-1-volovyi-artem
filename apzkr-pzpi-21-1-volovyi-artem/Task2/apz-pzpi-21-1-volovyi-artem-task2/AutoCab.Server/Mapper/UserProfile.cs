using AutoCab.Db.Models;
using AutoCab.Shared.Dto.Account;
using AutoMapper;

namespace AutoCab.Server.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserInfoDto>()
            .ForMember(o => o.Role, opt => opt.MapFrom(src => src.Role.Name));
    }
}
