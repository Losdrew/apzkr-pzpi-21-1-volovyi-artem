using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AutoCab.Server.Features.Car;

public class GetCarsForTripQuery : GetCarsForTripQueryDto, IRequest<ServiceResponse<ICollection<CarForTripDto>>>
{
    public class GetAvailableCarsQueryHandler : 
        ExtendedBaseHandler<GetCarsForTripQuery, ServiceResponse<ICollection<CarForTripDto>>>
    {
        private readonly IGeolocationService _geolocationService;

        public GetAvailableCarsQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, IGeolocationService geolocationService, ILogger<GetAvailableCarsQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
            _geolocationService = geolocationService;
        }

        public override async Task<ServiceResponse<ICollection<CarForTripDto>>> Handle(GetCarsForTripQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get cars for trip error");
                return ServiceResponseBuilder.Failure<ICollection<CarForTripDto>>(CarError.GetCarsError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<CarForTripDto>>> UnsafeHandleAsync(
            GetCarsForTripQuery request, CancellationToken cancellationToken)
        {
            var availableCars = await Context.Cars
                .Where(car => car.Status == CarStatus.Idle || car.Status == CarStatus.WaitingForPassenger)
                .ToListAsync(cancellationToken);

            var carsForTrip = new List<CarForTripDto>();
            foreach (var car in availableCars)
            {
                if (car.Location == null)
                {
                    continue;
                }

                var carLocation = new LocationDto { X = car.Location.X, Y = car.Location.Y };
                var startLocation = await _geolocationService.GetAddressLocationAsync(request.StartAddress);
                
                if (!startLocation.IsSuccess)
                {
                    continue;
                }

                var routeToStartLocation = await _geolocationService.GetRoutesAsync(carLocation, startLocation.Result);

                if (!routeToStartLocation.IsSuccess)
                {
                    continue;
                }

                var distanceToStartLocation = routeToStartLocation.Result.Routes[0].Distance;

                var endLocation = await _geolocationService.GetAddressLocationAsync(request.DestinationAddress);

                if (!endLocation.IsSuccess)
                {
                    continue;
                }

                var routeToDestinationLocation = await _geolocationService.GetRoutesAsync(startLocation.Result, endLocation.Result);

                if (!routeToDestinationLocation.IsSuccess)
                {
                    continue;
                }

                var distanceToDestinationLocation = routeToDestinationLocation.Result.Routes[0].Distance;

                decimal totalDistance = Convert.ToDecimal(distanceToStartLocation + distanceToDestinationLocation);

                var carForTrip = Mapper.Map<CarForTripDto>(car);
                carForTrip.Price = CalculatePrice(totalDistance);
                carsForTrip.Add(carForTrip);
            }

            var sortedCars = carsForTrip.OrderBy(car => car.Price).ToList();
            var result = new Collection<CarForTripDto>(sortedCars);
            return ServiceResponseBuilder.Success((ICollection<CarForTripDto>)result);
        }

        private decimal CalculatePrice(decimal distance)
        {
            decimal pricePerM = 0.025M;
            return distance * pricePerM;
        }
    }
}