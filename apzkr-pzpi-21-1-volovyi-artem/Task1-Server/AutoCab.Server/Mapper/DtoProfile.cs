using AutoMapper;
using AutoCab.Shared.Dto.Error;
using AutoCab.Shared.Errors.Base;

namespace AutoCab.Server.Mapper;

public class DtoProfile : Profile
{
    public DtoProfile()
    {
        CreateMap<ServiceError, ServiceErrorDto>();
        CreateMap<Error, ErrorDto>()
            .ForMember(e => e.ServiceErrors, otp => otp.MapFrom(src => src.ServiceErrors));
    }
}