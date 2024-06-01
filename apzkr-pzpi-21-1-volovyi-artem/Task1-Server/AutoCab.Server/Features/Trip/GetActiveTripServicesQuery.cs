using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Service;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Trip;

public class GetActiveTripServicesQuery : IRequest<ServiceResponse<ServicesCommandsDto>>
{
    public string DeviceId { get; set; }

    public class GetActiveTripServicesQueryHandler : ExtendedBaseHandler<GetActiveTripServicesQuery, ServiceResponse<ServicesCommandsDto>>
    {
        public GetActiveTripServicesQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetActiveTripServicesQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ServicesCommandsDto>> Handle(GetActiveTripServicesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get active trip services error");
                return ServiceResponseBuilder.Failure<ServicesCommandsDto>(TripServiceError.GetServicesError);
            }
        }

        protected override async Task<ServiceResponse<ServicesCommandsDto>> UnsafeHandleAsync(
            GetActiveTripServicesQuery request, CancellationToken cancellationToken)
        {
            var trip = await Context.Trips
                .Include(t => t.Services)
                .Include(t => t.Car)
                .FirstOrDefaultAsync(t => t.Car.DeviceId.Equals(request.DeviceId), cancellationToken);

            if (trip == null || trip.Status == TripStatus.Completed || trip.Status == TripStatus.Cancelled)
            {
                return ServiceResponseBuilder.Failure<ServicesCommandsDto>(TripError.TripNotFound);
            }

            var result = new ServicesCommandsDto()
            {
                Commands = trip.Services.Select(s => s.Command).ToList()
            };
            return ServiceResponseBuilder.Success(result);
        }
    }
}
