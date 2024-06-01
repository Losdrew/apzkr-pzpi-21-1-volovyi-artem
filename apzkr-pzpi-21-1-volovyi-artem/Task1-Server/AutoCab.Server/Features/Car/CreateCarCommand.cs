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

public class CreateCarCommand : CreateCarCommandDto, IRequest<ServiceResponse<CarInfoDto>>
{
    public class CreateCarCommandHandler :
        ExtendedBaseHandler<CreateCarCommand, ServiceResponse<CarInfoDto>>
    {
        public CreateCarCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<CreateCarCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<CarInfoDto>> Handle(CreateCarCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Car creation error");
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.CarCreateError);
            }
        }

        protected override async Task<ServiceResponse<CarInfoDto>> UnsafeHandleAsync(CreateCarCommand request,
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

            var car = Mapper.Map<Db.Models.Car>(request);
            Context.Add(car);
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<CarInfoDto>(car);
            return ServiceResponseBuilder.Success(result);
        }
    }
}