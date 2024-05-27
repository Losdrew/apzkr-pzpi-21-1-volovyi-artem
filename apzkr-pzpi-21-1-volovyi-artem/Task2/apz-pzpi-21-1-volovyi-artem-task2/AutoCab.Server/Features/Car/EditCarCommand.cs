using AutoCab.Db.DbContexts;
using AutoCab.Server.Extensions;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.Helpers;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Car;

public class EditCarCommand : EditCarCommandDto, IRequest<ServiceResponse<CarInfoDto>>
{
    public class EditCarCommandHandler :
        ExtendedBaseHandler<EditCarCommand, ServiceResponse<CarInfoDto>>
    {
        public EditCarCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<EditCarCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<CarInfoDto>> Handle(EditCarCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Edit car error");
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarEditError);
            }
        }

        protected override async Task<ServiceResponse<CarInfoDto>> UnsafeHandleAsync(EditCarCommand request,
            CancellationToken cancellationToken)
        {
            var isUserIdValid = ContextAccessor.TryGetUserId(out var userId);
            
            var administrator = await Context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (!isUserIdValid || administrator == null || administrator?.Role?.Name != Roles.Administrator)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(UserError.InvalidAuthorization);
            }

            var carToEdit = await Context.Cars.FindAsync(request.Id);

            if (carToEdit == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarNotFound);
            }

            Mapper.Map(request, carToEdit);
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<CarInfoDto>(carToEdit);
            return ServiceResponseBuilder.Success(result);
        }
    }
}