using AutoMapper;
using AutoCab.Db.Models;
using AutoCab.Shared.Dto.Address;

namespace AutoCab.Server.Mapper;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>()
            .ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null));
    }
}