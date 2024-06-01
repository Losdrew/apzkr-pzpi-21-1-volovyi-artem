using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Account;

public class DeleteUserCommand : IRequest<ServiceResponse>
{
    public Guid UserId { get; set; }

    public class DeleteUserCommandHandler : BaseHandler<DeleteUserCommand, ServiceResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;
        protected readonly ApplicationDbContext Context;

        public DeleteUserCommandHandler(UserManager<IdentityUser> userManager,
            ApplicationDbContext context, ILogger<DeleteUserCommandHandler> logger)
            : base(logger)
        {
            _userManager = userManager;
            Context = context;
        }

        public override async Task<ServiceResponse> Handle(DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Delete user error");
                return ServiceResponseBuilder.Failure(UserError.UserDeleteError);
            }
        }

        protected override async Task<ServiceResponse> UnsafeHandleAsync(DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var userToDelete = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (userToDelete == null)
            {
                return ServiceResponseBuilder.Failure(UserError.UserNotFound);
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            var dbUserToDelete = Context.Users.Where(u => u.Id == request.UserId);
            var isDeleted = await dbUserToDelete.ExecuteDeleteAsync(cancellationToken);
            await dbUserToDelete.ExecuteDeleteAsync(cancellationToken);

            if (!result.Succeeded || isDeleted == 0)
            {
                return ServiceResponseBuilder.Failure(UserError.UserDeleteError);
            }

            return ServiceResponseBuilder.Success();
        }
    }
}
