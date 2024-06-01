using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Trip;

public class GetActiveTripLocationQuery : IRequest<ServiceResponse<LocationDto>>
{
    public string DeviceId { get; set; }

    public class GetActiveTripLocationQueryHandler 
        : ExtendedBaseHandler<GetActiveTripLocationQuery, ServiceResponse<LocationDto>>
    {
        private readonly IGeolocationService _geolocationService;

        public GetActiveTripLocationQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetActiveTripLocationQueryHandler> logger, IGeolocationService geolocationService)
            : base(context, contextAccessor, mapper, logger)
        {
            _geolocationService = geolocationService;
        }

        public override async Task<ServiceResponse<LocationDto>> Handle(
            GetActiveTripLocationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get active trip location error");
                return ServiceResponseBuilder.Failure<LocationDto>(TripError.GetTripError);
            }
        }

        protected override async Task<ServiceResponse<LocationDto>> UnsafeHandleAsync(
            GetActiveTripLocationQuery request, CancellationToken cancellationToken)
        {
            var trip = await Context.Trips
                .Include(t => t.StartAddress)
                .Include(t => t.DestinationAddress)
                .Include(t => t.Car)
                .FirstOrDefaultAsync(t => t.Car.DeviceId.Equals(request.DeviceId), cancellationToken);

            if (trip == null || trip.Status == TripStatus.Completed || trip.Status == TripStatus.Cancelled)
            {
                return ServiceResponseBuilder.Failure<LocationDto>(TripError.TripNotFound);
            }

            AddressDto address = new AddressDto(); 

            if (trip.Status == TripStatus.Created)
            {
                address = Mapper.Map<AddressDto>(trip.StartAddress);
            }
            if (trip.Status == TripStatus.InProgress)
            {
                address = Mapper.Map<AddressDto>(trip.DestinationAddress);
            }

            var location = await _geolocationService.GetAddressLocationAsync(address);

            if (!location.IsSuccess)
            {
                return ServiceResponseBuilder.Failure<LocationDto>(location.Error);
            }

            return ServiceResponseBuilder.Success(location.Result);
        }
    }
}