using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Trip;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Trip;

public class UpdateTripServicesCommand : UpdateTripServicesCommandDto, IRequest<ServiceResponse<TripInfoDto>>
{
    public class UpdateTripServicesCommandHandler 
        : ExtendedBaseHandler<UpdateTripServicesCommand, ServiceResponse<TripInfoDto>>
    {
        public UpdateTripServicesCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<UpdateTripServicesCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<TripInfoDto>> Handle(UpdateTripServicesCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Trip services update error");
                return ServiceResponseBuilder.Failure<TripInfoDto>(TripError.TripUpdateError);
            }
        }

        protected override async Task<ServiceResponse<TripInfoDto>> UnsafeHandleAsync(UpdateTripServicesCommand request,
            CancellationToken cancellationToken)
        {
            var trip = await Context.Trips
                .Include(t => t.Services)
                .Include(t => t.StartAddress)
                .Include(t => t.DestinationAddress)
                .FirstOrDefaultAsync(t => t.Id == request.TripId, cancellationToken);

            if (trip == null)
            {
                return ServiceResponseBuilder.Failure<TripInfoDto>(TripError.TripNotFound);
            }

            trip.Services.Clear();

            foreach (var serviceId in request.Services)
            {
                var service = await Context.Services.FindAsync(serviceId);
                if (service == null)
                {
                    return ServiceResponseBuilder.Failure<TripInfoDto>(TripServiceError.ServiceNotFound);
                }
                trip.Services.Add(service);
            }

            await Context.SaveChangesAsync(cancellationToken);
            var result = Mapper.Map<TripInfoDto>(trip);
            return ServiceResponseBuilder.Success(result);
        }
    }
}
