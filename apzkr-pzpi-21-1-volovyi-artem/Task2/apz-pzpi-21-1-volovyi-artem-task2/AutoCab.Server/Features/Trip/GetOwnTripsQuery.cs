using AutoMapper;
using AutoCab.Db.DbContexts;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Trip;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Trip;

public class GetOwnTripsQuery : IRequest<ServiceResponse<ICollection<TripInfoDto>>>
{
    public class GetOwnTripsQueryHandler :
        ExtendedBaseHandler<GetOwnTripsQuery, ServiceResponse<ICollection<TripInfoDto>>>
    {
        public GetOwnTripsQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetOwnTripsQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<TripInfoDto>>> Handle(GetOwnTripsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get own trips error");
                return ServiceResponseBuilder.Failure<ICollection<TripInfoDto>>(TripError.GetOwnTripsError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<TripInfoDto>>> UnsafeHandleAsync(
            GetOwnTripsQuery request, CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            var customer = await Context.Users.FindAsync(userId);

            if (!isUserIdValid || customer == null)
            {
                return ServiceResponseBuilder.Failure<ICollection<TripInfoDto>>(UserError.InvalidAuthorization);
            }

            var trip = Context.Trips
                .Include(t => t.StartAddress)
                .Include(t => t.DestinationAddress)
                .Include(t => t.Services)
                .Where(t => t.UserId == customer.Id);

            var result = Mapper.Map<ICollection<TripInfoDto>>(trip);
            return ServiceResponseBuilder.Success(result);
        }
    }
}