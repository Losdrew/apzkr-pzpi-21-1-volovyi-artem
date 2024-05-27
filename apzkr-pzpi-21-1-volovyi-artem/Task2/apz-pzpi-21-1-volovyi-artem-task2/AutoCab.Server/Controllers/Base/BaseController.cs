using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using AutoCab.Shared.Dto.Error;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutoCab.Server.Controllers.Base;

public abstract class BaseController : ControllerBase
{
    private readonly IMapper _mapper;
    protected readonly IMediator Mediator;

    protected BaseController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        Mediator = mediator;
    }

    protected IActionResult ConvertFromServiceResponse(ServiceResponse serviceResponse)
    {
        if (serviceResponse.IsSuccess)
        {
            return Ok();
        }

        var errorDto = _mapper.Map<ErrorDto>(serviceResponse.Error);
        return BadRequest(errorDto);
    }

    protected IActionResult ConvertFromServiceResponse<T>(ServiceResponse<T> serviceResponse)
    {
        if (serviceResponse.IsSuccess)
        {
            return Ok(serviceResponse.Result);
        }

        var errorDto = _mapper.Map<ErrorDto>(serviceResponse.Error);
        return BadRequest(errorDto);
    }
}