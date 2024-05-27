using AutoMapper;
using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Trip;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Trip;

public class CancelOwnTripCommand : IRequest<ServiceResponse<TripInfoDto>>
{
    public Guid TripId { get; set; }

    public class CancelOwnTripCommandHandler :
        ExtendedBaseHandler<CancelOwnTripCommand, ServiceResponse<TripInfoDto>>
    {
        public CancelOwnTripCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<CancelOwnTripCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<TripInfoDto>> Handle(CancelOwnTripCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Cancel trip error");
                return ServiceResponseBuilder.Failure<TripInfoDto>(TripError.TripCancelError);
            }
        }

        protected override async Task<ServiceResponse<TripInfoDto>> UnsafeHandleAsync(CancelOwnTripCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            var customer = await Context.Users.FindAsync(userId);

            if (!isUserIdValid || customer == null)
            {
                return ServiceResponseBuilder.Failure<TripInfoDto>(UserError.InvalidAuthorization);
            }

            var tripToCancel = await Context.Trips
                .Include(o => o.StartAddress)
                .Include(o => o.DestinationAddress)
                .FirstOrDefaultAsync(o => o.Id == request.TripId, cancellationToken);

            if (tripToCancel == null)
            {
                return ServiceResponseBuilder.Failure<TripInfoDto>(TripError.TripNotFound);
            }

            if (tripToCancel.UserId != customer.Id)
            {
                return ServiceResponseBuilder.Failure<TripInfoDto>(UserError.ForbiddenAccess);
            }

            tripToCancel.Status = TripStatus.Cancelled;
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<TripInfoDto>(tripToCancel);
            return ServiceResponseBuilder.Success(result);
        }
    }
}