using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.ServiceResponseHandling;

namespace AutoCab.Server.Services;

public interface IGeolocationService
{
    public Task<ServiceResponse<LocationDto>> GetAddressLocationAsync(AddressDto address);
    public Task<ServiceResponse<RoutesDto>> GetRoutesAsync(LocationDto firstPoint, LocationDto secondPoint);
    public Task<ServiceResponse<RoutesDto>> GetNearestGasStationRoute(LocationDto currentLocation);
}