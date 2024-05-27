using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class GetCarStatusQuery : IRequest<ServiceResponse<string>>
{
    public string? DeviceId { get; set; }

    public class GetCarStatusQueryHandler : ExtendedBaseHandler<GetCarStatusQuery, ServiceResponse<string>>
    {
        public GetCarStatusQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetCarStatusQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<string>> Handle(GetCarStatusQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get car status error");
                return ServiceResponseBuilder.Failure<string>(CarError.CarUnavailableError);
            }
        }

        protected override async Task<ServiceResponse<string>> UnsafeHandleAsync(GetCarStatusQuery request,
            CancellationToken cancellationToken)
        {
            var car = Context.Cars.FirstOrDefault(r => r.DeviceId != null && r.DeviceId.Equals(request.DeviceId));

            if (car == null)
            {
                return ServiceResponseBuilder.Failure<string>(CarError.CarNotFound);
            }

            return ServiceResponseBuilder.Success(((int)car.Status).ToString());
        }
    }
}