using AutoMapper;
using AutoCab.Db.DbContexts;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Trip;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoCab.Shared.Dto.Car;
using System.Collections.ObjectModel;

namespace AutoCab.Server.Features.Trip;

public class GetTripsQuery : IRequest<ServiceResponse<ICollection<TripInfoDto>>>
{
    public class GetTripsQueryHandler :
        ExtendedBaseHandler<GetTripsQuery, ServiceResponse<ICollection<TripInfoDto>>>
    {
        public GetTripsQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetTripsQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<TripInfoDto>>> Handle(GetTripsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get trips error");
                return ServiceResponseBuilder.Failure<ICollection<TripInfoDto>>(TripError.GetTripError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<TripInfoDto>>> UnsafeHandleAsync(
            GetTripsQuery request, CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            var admin = await Context.Users.FindAsync(userId);

            if (!isUserIdValid || admin == null)
            {
                return ServiceResponseBuilder.Failure<ICollection<TripInfoDto>>(UserError.InvalidAuthorization);
            }

            var trips = new List<TripInfoDto>();
            var users = Context.Users.ToList();

            foreach (var user in users)
            {
                var trip = Context.Trips
                    .Include(t => t.StartAddress)
                    .Include(t => t.DestinationAddress)
                    .Include(t => t.Services)
                    .Where(t => t.UserId == user.Id)
                    .ToList();

                if (trip.Any())
                {
                    var tripInfo = Mapper.Map<List<TripInfoDto>>(trip);
                    trips.AddRange(tripInfo);
                }
            }

            var result = new Collection<TripInfoDto>(trips);
            return ServiceResponseBuilder.Success((ICollection<TripInfoDto>)result);
        }
    }
}