using AutoCab.Server.Models.Geolocation;
using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Dto.Geolocation;
using AutoCab.Shared.Errors.Base;
using AutoCab.Shared.ServiceResponseHandling;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace AutoCab.Server.Services;

public class GeolocationService : IGeolocationService
{
    private const string PositionStackBaseUrl = "http://api.positionstack.com/v1/forward";
    private const string MapboxBaseUrl = "https://api.mapbox.com";

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GeolocationService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<ServiceResponse<LocationDto>> GetAddressLocationAsync(AddressDto address)
    {
        var accessToken = _configuration.GetRequiredSection("PositionStackAccessToken").Value;
        var fullAddress = GetFullAddress(address);
        var endpoint = $"{PositionStackBaseUrl}?access_key={accessToken}&query={fullAddress}";

        var httpResponseMessage = await _httpClient.GetAsync(endpoint);
        var json = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return HandleError(json).MapErrorResult<LocationDto>();
        }

        var result = JsonConvert.DeserializeObject<GeolocationResult>(json);
        var addressResult = result.Data.First();
        var location = new LocationDto
        {
            X = addressResult.Longitude,
            Y = addressResult.Latitude
        };

        return ServiceResponseBuilder.Success(location);
    }

    public async Task<ServiceResponse<RoutesDto>> GetRoutesAsync(LocationDto firstPoint, LocationDto secondPoint)
    {
        var accessToken = _configuration.GetRequiredSection("MapBoxAccessToken").Value;
        var location = string.Format(CultureInfo.InvariantCulture, "{0},{1};{2},{3}", 
            firstPoint.X, firstPoint.Y, secondPoint.X, secondPoint.Y);
        var parameters = $"geometries=geojson&overview=simplified&steps=true&access_token={accessToken}";
        var endpoint = $"{MapboxBaseUrl}/directions/v5/mapbox/driving/{location}?{parameters}";

        var httpResponseMessage = await _httpClient.GetAsync(endpoint);
        var json = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return HandleError(json).MapErrorResult<RoutesDto>();
        }

        var result = JsonConvert.DeserializeObject<RoutesDto>(json);
        return ServiceResponseBuilder.Success(result);
    }

    public async Task<ServiceResponse<RoutesDto>> GetNearestGasStationRoute(LocationDto currentLocation)
    {
        var accessToken = _configuration.GetRequiredSection("MapBoxAccessToken").Value;
        var location = string.Format(CultureInfo.InvariantCulture, "{0},{1}", currentLocation.X, currentLocation.Y);
        var parameters = $"search/searchbox/v1/suggest?q=gas+station&language=en&limit=1&" +
            $"navigation_profile=driving&origin={location}&session_token=&access_token={accessToken}";
        var endpoint = $"{MapboxBaseUrl}/{parameters}";

        var httpResponseMessage = await _httpClient.GetAsync(endpoint);
        var json = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return HandleError(json).MapErrorResult<RoutesDto>();
        }

        var suggestionResult = JsonConvert.DeserializeObject<SuggestionResult>(json);

        var mapboxId = suggestionResult.Suggestions.First().Mapbox_Id;

        parameters = $"search/searchbox/v1/retrieve/{mapboxId}?session_token=&access_token={accessToken}";
        endpoint = $"{MapboxBaseUrl}/{parameters}";

        httpResponseMessage = await _httpClient.GetAsync(endpoint);
        json = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return HandleError(json).MapErrorResult<RoutesDto>();
        }

        var featureResult = JsonConvert.DeserializeObject<FeatureResult>(json);

        var featureCoordinates = featureResult.Features.First().Geometry.Coordinates;

        var gasStationLocation = new LocationDto
        {
            X = featureCoordinates[0],
            Y = featureCoordinates[1]
        };

        var route = await GetRoutesAsync(currentLocation, gasStationLocation);

        return ServiceResponseBuilder.Success(route.Result);
    }

    private string GetFullAddress(AddressDto address)
    {
        var fullAddress = new StringBuilder();

        fullAddress.Append(address.AddressLine1 + " ");
        fullAddress.Append(address.AddressLine2 + " ");

        if (!string.IsNullOrEmpty(address.AddressLine3))
        {
            fullAddress.Append(address.AddressLine3 + " ");
        }

        if (!string.IsNullOrEmpty(address.AddressLine4))
        {
            fullAddress.Append(address.AddressLine4 + " ");
        }

        fullAddress.Append(", " + address.TownCity);
        fullAddress.Append(", " + address.Region);
        fullAddress.Append(", " + address.Country);

        return fullAddress.ToString();
    }

    private ServiceResponse HandleError(string json)
    {
        var errorResult = JsonConvert.DeserializeObject<GeolocationError>(json);
        var error = new Error().ServiceErrors = new List<ServiceError>
        {
            new()
            {
                Code = 400,
                Header = errorResult?.Error?.Code,
                ErrorMessage = errorResult?.Error?.Message
            }
        };

        return ServiceResponseBuilder.Failure(error);
    }
}