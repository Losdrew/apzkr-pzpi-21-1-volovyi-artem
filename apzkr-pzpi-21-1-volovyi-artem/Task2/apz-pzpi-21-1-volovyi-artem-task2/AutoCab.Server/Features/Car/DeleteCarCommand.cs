using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Car;

public class DeleteCarCommand : IRequest<ServiceResponse>
{
    public Guid CarId { get; set; }

    public class DeleteCarCommandHandler :
        ExtendedBaseHandler<DeleteCarCommand, ServiceResponse>
    {
        public DeleteCarCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<DeleteCarCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse> Handle(DeleteCarCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Delete car error");
                return ServiceResponseBuilder.Failure(CarError.CarDeleteError);
            }
        }

        protected override async Task<ServiceResponse> UnsafeHandleAsync(DeleteCarCommand request,
            CancellationToken cancellationToken)
        {
            var carToDelete = Context.Cars.Where(p => p.Id == request.CarId);
            var isDeleted = await carToDelete.ExecuteDeleteAsync(cancellationToken);

            if (isDeleted == 0)
            {
                return ServiceResponseBuilder.Failure(CarError.CarNotFound);
            }

            return ServiceResponseBuilder.Success();
        }
    }
}