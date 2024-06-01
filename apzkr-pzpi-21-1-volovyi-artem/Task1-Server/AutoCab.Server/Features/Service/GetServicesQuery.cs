using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Service;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Service;

public class GetServicesQuery : IRequest<ServiceResponse<ICollection<ServiceInfoDto>>>
{
    public class GetServicesQueryHandler : ExtendedBaseHandler<GetServicesQuery, ServiceResponse<ICollection<ServiceInfoDto>>>
    {
        public GetServicesQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetServicesQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<ServiceInfoDto>>> Handle(GetServicesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get services error");
                return ServiceResponseBuilder.Failure<ICollection<ServiceInfoDto>>(TripServiceError.GetServicesError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<ServiceInfoDto>>> UnsafeHandleAsync(
            GetServicesQuery request, CancellationToken cancellationToken)
        {
            var services = Context.Services;
            var result = Mapper.Map<ICollection<ServiceInfoDto>>(services);
            return ServiceResponseBuilder.Success(result);
        }
    }
}
