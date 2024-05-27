using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class UpdateCarCommand : UpdateCarCommandDto, IRequest<ServiceResponse>
{
    public class UpdateCarCommandHandler : ExtendedBaseHandler<UpdateCarCommand, ServiceResponse>
    {
        public UpdateCarCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<UpdateCarCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse> Handle(UpdateCarCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Update car error");
                return ServiceResponseBuilder.Failure(CarError.CarEditError);
            }
        }

        protected override async Task<ServiceResponse> UnsafeHandleAsync(UpdateCarCommand request,
            CancellationToken cancellationToken)
        {
            var car = Context.Cars.FirstOrDefault(r => r.DeviceId != null && r.DeviceId.Equals(request.DeviceId));

            if (car == null)
            {
                return ServiceResponseBuilder.Failure(CarError.CarNotFound);
            }

            Mapper.Map(request, car);

            var trip = Context.Trips.FirstOrDefault(d => d.CarId == car.Id);

            if (trip != null)
            {
                trip.Status = car.Status switch
                {
                    CarStatus.EnRoute => TripStatus.InProgress,
                    CarStatus.OnTrip => TripStatus.InProgress,
                    CarStatus.WaitingForPassenger => TripStatus.WaitingForPassenger,
                    _ => trip.Status
                };
            }

            await Context.SaveChangesAsync(cancellationToken);
            return ServiceResponseBuilder.Success();
        }
    }
}