using AutoCab.Db.DbContexts;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Service;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.Helpers;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Service;

public class EditServiceCommand : ServiceDto, IRequest<ServiceResponse<ServiceInfoDto>>
{
    public Guid Id { get; set; }

    public class EditServiceCommandHandler : ExtendedBaseHandler<EditServiceCommand, ServiceResponse<ServiceInfoDto>>
    {
        public EditServiceCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<EditServiceCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ServiceInfoDto>> Handle(EditServiceCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Edit service error");
                return ServiceResponseBuilder.Failure<ServiceInfoDto>(TripServiceError.ServiceEditError);
            }
        }

        protected override async Task<ServiceResponse<ServiceInfoDto>> UnsafeHandleAsync(EditServiceCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);

            var administrator = await Context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (!isUserIdValid || administrator == null || administrator?.Role?.Name != Roles.Administrator)
            {
                return ServiceResponseBuilder.Failure<ServiceInfoDto>(UserError.InvalidAuthorization);
            }

            var serviceToEdit = await Context.Services.FindAsync(request.Id);

            if (serviceToEdit == null)
            {
                return ServiceResponseBuilder.Failure<ServiceInfoDto>(TripServiceError.ServiceNotFound);
            }

            Mapper.Map(request, serviceToEdit);
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<ServiceInfoDto>(serviceToEdit);
            return ServiceResponseBuilder.Success(result);
        }
    }
}
