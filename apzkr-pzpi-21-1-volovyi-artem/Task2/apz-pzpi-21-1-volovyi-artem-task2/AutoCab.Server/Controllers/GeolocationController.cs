using AutoCab.Server.Controllers.Base;
using AutoCab.Server.Features.Geolocation;
using AutoCab.Shared.Dto.Error;
using AutoCab.Shared.Dto.Geolocation;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AutoCab.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeolocationController : BaseController
{
    public GeolocationController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    /// <summary>
    /// Get routes from one location to another.
    /// </summary>
    /// <remarks>
    /// If the operation is successful, it will return a RoutesDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <param name="coordinates">The coordinates of two points separated by semicolon</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpGet("route/{coordinates}")]
    [ProducesResponseType(typeof(RoutesDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> GetRoute([FromRoute] string coordinates)
    {
        var points = coordinates.Split(';');

        var query = new GetRouteQuery
        {
            FirstPoint = ParseLocation(points[0]),
            SecondPoint = ParseLocation(points[1])
        };

        var result = await Mediator.Send(query);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Get route to the nearest gas station.
    /// </summary>
    /// <remarks>
    /// If the operation is successful, it will return a RoutesDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <param name="location">The origin location</param>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpGet("gas-station-route/{location}")]
    [ProducesResponseType(typeof(RoutesDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> GetNearestGasStationRoute([FromRoute] string location)
    {
        var query = new GetGasStationRouteQuery
        {
            Location = ParseLocation(location),
        };

        var result = await Mediator.Send(query);
        return ConvertFromServiceResponse(result);
    }

    private LocationDto ParseLocation(string locationString)
    {
        var location = new LocationDto();
        var coordinates = locationString.Split(',');

        location.X = double.Parse(coordinates[0], CultureInfo.InvariantCulture);
        location.Y = double.Parse(coordinates[1], CultureInfo.InvariantCulture);

        return location;
    }
}