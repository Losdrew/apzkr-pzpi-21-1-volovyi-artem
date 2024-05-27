using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Geolocation;

public class GetRouteQuery : GetRouteQueryDto, IRequest<ServiceResponse<RoutesDto>>
{
    public class GetRouteQueryHandler : ExtendedBaseHandler<GetRouteQuery, ServiceResponse<RoutesDto>>
    {
        private readonly IGeolocationService _geolocationService;

        public GetRouteQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetRouteQueryHandler> logger, IGeolocationService geolocationService)
            : base(context, contextAccessor, mapper, logger)
        {
            _geolocationService = geolocationService;
        }

        public override async Task<ServiceResponse<RoutesDto>> Handle(GetRouteQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get route error");
                return ServiceResponseBuilder.Failure<RoutesDto>(GeolocationError.GetRouteError);
            }
        }

        protected override async Task<ServiceResponse<RoutesDto>> UnsafeHandleAsync(GetRouteQuery request,
            CancellationToken cancellationToken)
        {
            var route = await _geolocationService.GetRoutesAsync(request.FirstPoint, request.SecondPoint);

            if (!route.IsSuccess)
            {
                return ServiceResponseBuilder.Failure<RoutesDto>(route.Error);
            }

            return ServiceResponseBuilder.Success(route.Result);
        }
    }
}