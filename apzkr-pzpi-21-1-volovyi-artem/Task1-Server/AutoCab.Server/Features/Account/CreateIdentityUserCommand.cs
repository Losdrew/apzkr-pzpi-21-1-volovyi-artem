using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Features.Base;
using AutoCab.Server.Models.Account;
using AutoCab.Shared.Dto.Account;
using AutoCab.Shared.Errors.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Account;

public class CreateIdentityUserCommand : CredentialsDto, IRequest<ServiceResponse<CreateIdentityUserResult>>
{
    public string Role { get; set; }

    public class CreateIdentityUserCommandHandler :
        BaseHandler<CreateIdentityUserCommand, ServiceResponse<CreateIdentityUserResult>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        protected readonly ApplicationDbContext Context;

        public CreateIdentityUserCommandHandler(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context, 
            ILogger<CreateIdentityUserCommandHandler> logger)
            : base(logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            Context = context;
        }

        public override async Task<ServiceResponse<CreateIdentityUserResult>> Handle(CreateIdentityUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "User creation error");
                return ServiceResponseBuilder.Failure<CreateIdentityUserResult>(AccountError.AccountCreateError);
            }
        }

        protected override async Task<ServiceResponse<CreateIdentityUserResult>> UnsafeHandleAsync(
            CreateIdentityUserCommand request, CancellationToken cancellationToken)
        {
            var identityUser = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            var createResult = await _userManager.CreateAsync(identityUser, request.Password);

            if (createResult.Succeeded)
            {
                var roleId = await AssureRoleCreatedAsync(request.Role);
                await _userManager.AddToRoleAsync(identityUser, request.Role);

                var result = new CreateIdentityUserResult
                {
                    IdentityUser = identityUser,
                    RoleId = roleId
                };

                return ServiceResponseBuilder.Success(result);
            }

            var errors = createResult.Errors.Select(e => new ServiceError
            {
                Header = "UserError",
                ErrorMessage = e.Description,
                Code = int.Parse(e.Code)
            }).ToList();

            return ServiceResponseBuilder.Failure<CreateIdentityUserResult>(errors);
        }

        private async Task<Guid> AssureRoleCreatedAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));

                var dbRole = new Role() 
                { 
                    Name = role 
                };
                Context.Roles.Add(dbRole);
                await Context.SaveChangesAsync();
                return dbRole.Id;
            }
            var existingRole = await Context.Roles.FirstOrDefaultAsync(r => r.Name == role);
            return existingRole.Id;
        }
    }
}