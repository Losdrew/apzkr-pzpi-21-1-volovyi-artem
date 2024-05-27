using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Car;

public class ToggleCarDoorCommand : IRequest<ServiceResponse<CarInfoDto>>
{
    public Guid CarId { get; set; }

    public class ToggleCarDoorCommandHandler :
        ExtendedBaseHandler<ToggleCarDoorCommand, ServiceResponse<CarInfoDto>>
    {
        public ToggleCarDoorCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<ToggleCarDoorCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<CarInfoDto>> Handle(ToggleCarDoorCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Toggle car door error");
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarEditError);
            }
        }

        protected override async Task<ServiceResponse<CarInfoDto>> UnsafeHandleAsync(ToggleCarDoorCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);

            if (!isUserIdValid)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(UserError.InvalidAuthorization);
            }

            var user = await Context.Users.FindAsync(userId);

            if (user == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(UserError.InvalidAuthorization);
            }

            var carToEdit = await Context.Cars.FindAsync(request.CarId);

            if (carToEdit == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarNotFound);
            }

            Db.Models.Trip? trip;

            if (user != null)
            {
                trip = Context.Trips.FirstOrDefault(t => t.UserId == user.Id);

                if (trip == null || trip.Status != TripStatus.WaitingForPassenger)
                {
                    return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarUnavailableError);
                }

                carToEdit.IsDoorOpen = !carToEdit.IsDoorOpen;
            }

            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<CarInfoDto>(carToEdit);
            return ServiceResponseBuilder.Success(result);
        }
    }
}