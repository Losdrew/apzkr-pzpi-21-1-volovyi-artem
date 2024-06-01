using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Models.Account;
using AutoCab.Server.Services;
using AutoCab.Shared.Dto.Account;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.Helpers;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;

namespace AutoCab.Server.Features.Account;

public class CreateCustomerCommand : CreateUserCommandDto, IRequest<ServiceResponse<AuthResultDto>>
{
    public class CreateCustomerCommandHandler :
        SignUpHandler<CreateCustomerCommand, ServiceResponse<AuthResultDto>>
    {
        public CreateCustomerCommandHandler(IMediator mediator, ApplicationDbContext context,
            ITokenGenerator tokenGenerator, ILogger<CreateCustomerCommandHandler> logger)
            : base(mediator, context, tokenGenerator, logger)
        {
        }

        public override async Task<ServiceResponse<AuthResultDto>> Handle(CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Customer creation error");
                return ServiceResponseBuilder.Failure<AuthResultDto>(AccountError.AccountCreateError);
            }
        }

        protected override string GetRole()
        {
            return Roles.Customer;
        }

        protected override async Task<User> CreateUserAsync(CreateCustomerCommand request,
            ServiceResponse<CreateIdentityUserResult> createIdentityResponse)
        {
            var customer = new User
            {
                Id = Guid.Parse(createIdentityResponse.Result.IdentityUser.Id),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                RoleId = createIdentityResponse.Result.RoleId
            };

            Context.Users.Add(customer);
            await Context.SaveChangesAsync();

            return customer;
        }
    }
}