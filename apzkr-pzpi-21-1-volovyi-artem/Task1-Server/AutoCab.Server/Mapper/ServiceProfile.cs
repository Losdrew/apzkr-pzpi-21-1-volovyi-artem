using AutoCab.Db.Models;
using AutoCab.Server.Features.Service;
using AutoCab.Shared.Dto.Service;
using AutoMapper;

namespace AutoCab.Server.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<CreateServiceCommand, Service>();
        CreateMap<EditServiceCommand, Service>();
        CreateMap<Service, ServiceInfoDto>();
    }
}