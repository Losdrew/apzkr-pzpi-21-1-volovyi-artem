using AutoCab.Server.Features.Base;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Account;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AutoCab.Server.Features.Account;

public class SignInCommand : CredentialsDto, IRequest<ServiceResponse<AuthResultDto>>
{
    public class SignInCommandHandler : BaseHandler<SignInCommand, ServiceResponse<AuthResultDto>>
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly UserManager<IdentityUser> _userManager;

        public SignInCommandHandler(UserManager<IdentityUser> userManager, ITokenGenerator tokenGenerator,
            ILogger<SignInCommandHandler> logger)
            : base(logger)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        public override async Task<ServiceResponse<AuthResultDto>> Handle(SignInCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Sign in error");
                return ServiceResponseBuilder.Failure<AuthResultDto>(AccountError.LoginServiceError);
            }
        }

        protected override async Task<ServiceResponse<AuthResultDto>> UnsafeHandleAsync(SignInCommand request,
            CancellationToken cancellationToken)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser is null)
            {
                return ServiceResponseBuilder.Failure<AuthResultDto>(AccountError.LoginServiceError);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, request.Password);

            if (!isPasswordValid)
            {
                return ServiceResponseBuilder.Failure<AuthResultDto>(AccountError.LoginServiceError);
            }

            var token = await _tokenGenerator.GenerateAsync(identityUser);
            var result = new AuthResultDto
            {
                UserId = Guid.Parse(identityUser.Id),
                Bearer = token,
                Role = (await _userManager.GetRolesAsync(identityUser)).First()
            };

            return ServiceResponseBuilder.Success(result);
        }
    }
}