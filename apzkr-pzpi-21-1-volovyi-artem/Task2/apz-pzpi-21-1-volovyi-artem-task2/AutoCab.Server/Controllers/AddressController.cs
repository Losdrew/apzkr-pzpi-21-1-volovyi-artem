using AutoCab.Server.Controllers.Base;
using AutoCab.Server.Features.Address;
using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Dto.Error;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutoCab.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : BaseController
{
    public AddressController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    /// <summary>
    /// Get a list of addresses.
    /// </summary>
    /// <remarks>
    /// If the operation is successful, it will return an ICollection of AddressDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpGet("addresses")]
    [ProducesResponseType(typeof(AddressDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> GetAddresses()
    {
        var query = new GetAddressesQuery();
        var result = await Mediator.Send(query);
        return ConvertFromServiceResponse(result);
    }
}