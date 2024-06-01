using AutoCab.Server.Controllers.Base;
using AutoCab.Server.Features.Service;
using AutoCab.Shared.Dto.Error;
using AutoCab.Shared.Dto.Service;
using AutoCab.Shared.Helpers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCab.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : BaseController
{
    public ServiceController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    /// <summary>
    /// Get a list of services.
    /// </summary>
    /// <remarks>
    /// If the operation is successful, it will return an ICollection of ServiceInfoDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpGet("services")]
    [ProducesResponseType(typeof(ServiceInfoDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> GetServices()
    {
        var query = new GetServicesQuery();
        var result = await Mediator.Send(query);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Create new service.
    /// </summary>
    /// <param name="request">The request to create new service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If the operation is successful, it will return a ServiceInfoDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("create")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(ServiceInfoDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> CreateService(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Edit existing service.
    /// </summary>
    /// <param name="request">The request to edit service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If the operation is successful, it will return an ServiceInfoDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("edit")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(ServiceInfoDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 403)]
    public async Task<IActionResult> EditService(EditServiceCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Delete existing service.
    /// </summary>
    /// <param name="serviceId">The request to delete service.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("delete")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> DeleteService(Guid serviceId, CancellationToken cancellationToken)
    {
        var command = new DeleteServiceCommand
        {
            ServiceId = serviceId
        };
        var result = await Mediator.Send(command, cancellationToken);
        return ConvertFromServiceResponse(result);
    }
}
