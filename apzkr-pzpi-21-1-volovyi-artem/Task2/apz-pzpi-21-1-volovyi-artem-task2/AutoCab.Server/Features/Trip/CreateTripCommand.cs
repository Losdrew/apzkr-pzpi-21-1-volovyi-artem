using AutoCab.Db.DbContexts;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Trip;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Trip;

public class CreateTripCommand : CreateTripCommandDto, IRequest<ServiceResponse<TripInfoDto>>
{
    public class CreateTripCommandHandler :
        ExtendedBaseHandler<CreateTripCommand, ServiceResponse<TripInfoDto>>
    {
        public CreateTripCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<CreateTripCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<TripInfoDto>> Handle(CreateTripCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Trip creation error");
                return ServiceResponseBuilder.Failure<TripInfoDto>(TripError.TripCreateError);
            }
        }

        protected override async Task<ServiceResponse<TripInfoDto>> UnsafeHandleAsync(CreateTripCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            var customer = await Context.Users.FindAsync(userId);

            if (!isUserIdValid || customer == null)
            {
                return ServiceResponseBuilder.Failure<TripInfoDto>(UserError.InvalidAuthorization);
            }

            var newTrip = Mapper.Map<Db.Models.Trip>(request);
            newTrip.User = customer;
            newTrip.StartDateTime = DateTime.UtcNow;
            newTrip.Status = Db.Models.TripStatus.Created;
            
            ValidateAddresses(ref newTrip);

            Context.Add(newTrip);

            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<TripInfoDto>(newTrip);
            result.UserId = customer.Id;

            return ServiceResponseBuilder.Success(result);
        }

        private void ValidateAddresses(ref Db.Models.Trip trip)
        {
            var existingStartAddress = Context.Addresses.Find(trip.StartAddress.Id);

            if (existingStartAddress != null)
            {
                trip.StartAddress = null;
            }

            var existingDestinationAddress = Context.Addresses.Find(trip.DestinationAddress.Id);

            if (existingDestinationAddress != null)
            {
                trip.DestinationAddress = null;
            }
        }
    }
}