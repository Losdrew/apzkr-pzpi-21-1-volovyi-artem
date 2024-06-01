using AutoMapper;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Trip;
using AutoCab.Shared.Dto.Trip;

namespace AutoCab.Server.Mapper;

public class TripProfile : Profile
{
    public TripProfile()
    {
        CreateMap<CreateTripCommand, Trip>();

        CreateMap<Trip, TripInfoDto>()
            .ForMember(o => o.TripStatus, opt => opt.MapFrom(src => src.Status));
    }
}