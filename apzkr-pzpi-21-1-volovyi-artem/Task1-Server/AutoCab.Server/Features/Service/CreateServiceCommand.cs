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

public class CreateServiceCommand : ServiceDto, IRequest<ServiceResponse<ServiceInfoDto>>
{
    public class CreateServiceCommandHandler :
        ExtendedBaseHandler<CreateServiceCommand, ServiceResponse<ServiceInfoDto>>
    {
        public CreateServiceCommandHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<CreateServiceCommandHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ServiceInfoDto>> Handle(CreateServiceCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Service creation error");
                return ServiceResponseBuilder.Failure<ServiceInfoDto>(TripServiceError.ServiceCreateError);
            }
        }

        protected override async Task<ServiceResponse<ServiceInfoDto>> UnsafeHandleAsync(CreateServiceCommand request,
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

            var service = Mapper.Map<Db.Models.Service>(request);
            Context.Add(service);
            await Context.SaveChangesAsync(cancellationToken);

            var result = Mapper.Map<ServiceInfoDto>(service);
            return ServiceResponseBuilder.Success(result);
        }
    }
}
