using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Service;

public class DeleteServiceCommand : IRequest<ServiceResponse>
{
    public Guid ServiceId { get; set; }

    public class DeleteServiceCommandHandler :
        ExtendedBaseHandler<DeleteServiceCommand, ServiceResponse>
    {
        public DeleteServiceCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<DeleteServiceCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse> Handle(DeleteServiceCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Delete service error");
                return ServiceResponseBuilder.Failure(TripServiceError.ServiceDeleteError);
            }
        }

        protected override async Task<ServiceResponse> UnsafeHandleAsync(DeleteServiceCommand request,
            CancellationToken cancellationToken)
        {
            var serviceToDelete = Context.Services.Where(p => p.Id == request.ServiceId);
            var isDeleted = await serviceToDelete.ExecuteDeleteAsync(cancellationToken);

            if (isDeleted == 0)
            {
                return ServiceResponseBuilder.Failure(TripServiceError.ServiceNotFound);
            }

            return ServiceResponseBuilder.Success();
        }
    }
}
