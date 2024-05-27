using AutoCab.Server.Controllers.Base;
using AutoCab.Server.Features.Account;
using AutoCab.Shared.Dto.Account;
using AutoCab.Shared.Dto.Error;
using AutoCab.Shared.Helpers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCab.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : BaseController
{
    public AccountController(IMapper mapper, IMediator mediator)
        : base(mapper, mediator)
    {
    }

    /// <summary>
    /// Create a new administrator account.
    /// </summary>
    /// <param name="request">The request to create an administrator account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If the operation is successful, it will return an AuthResultDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("administrator/create")]
    [ProducesResponseType(typeof(AuthResultDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> SignUpAdministrator(CreateAdministratorCommand request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Create a new customer account.
    /// </summary>
    /// <param name="request">The request to create a customer account.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If the operation is successful, it will return an AuthResultDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("customer/create")]
    [ProducesResponseType(typeof(AuthResultDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> SignUpCustomer(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Perform user login
    /// </summary>
    /// <param name="request">The request to perform user login</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If the operation is successful, it will return an AuthResultDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthResultDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> SignIn(SignInCommand request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request, cancellationToken);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Get a list of users.
    /// </summary>
    /// <remarks>
    /// If the operation is successful, it will return an ICollection of UserInfoDto.
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpGet("users")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(UserInfoDto), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> GetUsers()
    {
        var query = new GetUsersQuery();
        var result = await Mediator.Send(query);
        return ConvertFromServiceResponse(result);
    }

    /// <summary>
    /// Delete existing user.
    /// </summary>
    /// <param name="userId">The request to delete user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// If there is a bad request, it will return an ErrorDto.
    /// </remarks>
    /// <returns>An IActionResult representing the result of the operation.</returns>
    [HttpDelete("delete")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand
        {
            UserId = userId
        };
        var result = await Mediator.Send(command, cancellationToken);
        return ConvertFromServiceResponse(result);
    }
}