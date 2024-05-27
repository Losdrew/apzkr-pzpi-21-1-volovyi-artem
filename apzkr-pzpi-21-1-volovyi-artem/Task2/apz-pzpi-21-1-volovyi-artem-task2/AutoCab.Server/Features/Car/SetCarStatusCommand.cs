using AutoCab.Db.DbContexts;
using AutoCab.Db.Models;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class SetCarStatusCommand : SetCarStatusCommandDto, IRequest<ServiceResponse<CarInfoDto>>
{
    public class SetCarStatusCommandHandler : ExtendedBaseHandler<SetCarStatusCommand, ServiceResponse<CarInfoDto>>
    {
        public SetCarStatusCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<SetCarStatusCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<CarInfoDto>> Handle(SetCarStatusCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Set car status error");
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarEditError);
            }
        }

        protected override async Task<ServiceResponse<CarInfoDto>> UnsafeHandleAsync(SetCarStatusCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            var user = await Context.Users.FindAsync(userId);

            if (!isUserIdValid || user == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(UserError.InvalidAuthorization);
            }

            var carToEdit = await Context.Cars.FindAsync(request.Id);

            if (carToEdit == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarNotFound);
            }

            carToEdit.Status = (CarStatus)request.NewStatus;
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<CarInfoDto>(carToEdit);
            return ServiceResponseBuilder.Success(result);
        }
    }
}