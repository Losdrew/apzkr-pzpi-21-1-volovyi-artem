using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Geolocation;

public class GetGasStationRouteQuery : GetGasStationRouteQueryDto, IRequest<ServiceResponse<RoutesDto>>
{
    public class GetGasStationRouteQueryHandler : ExtendedBaseHandler<GetGasStationRouteQuery, ServiceResponse<RoutesDto>>
    {
        private readonly IGeolocationService _geolocationService;

        public GetGasStationRouteQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetGasStationRouteQueryHandler> logger, IGeolocationService geolocationService)
            : base(context, contextAccessor, mapper, logger)
        {
            _geolocationService = geolocationService;
        }

        public override async Task<ServiceResponse<RoutesDto>> Handle(GetGasStationRouteQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get gas station route error");
                return ServiceResponseBuilder.Failure<RoutesDto>(GeolocationError.GetRouteError);
            }
        }

        protected override async Task<ServiceResponse<RoutesDto>> UnsafeHandleAsync(GetGasStationRouteQuery request,
            CancellationToken cancellationToken)
        {
            var route = await _geolocationService.GetNearestGasStationRoute(request.Location);

            if (!route.IsSuccess)
            {
                return ServiceResponseBuilder.Failure<RoutesDto>(route.Error);
            }

            return ServiceResponseBuilder.Success(route.Result);
        }
    }
}