using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.Helpers;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class GetCarDoorStatusQuery : IRequest<ServiceResponse<string>>
{
    public string? DeviceId { get; set; }

    public class GetCarDoorStatusQueryHandler : 
        ExtendedBaseHandler<GetCarDoorStatusQuery, ServiceResponse<string>>
    {
        public GetCarDoorStatusQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetCarDoorStatusQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<string>> Handle(GetCarDoorStatusQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get car door status error");
                return ServiceResponseBuilder.Failure<string>(CarError.CarUnavailableError);
            }
        }

        protected override async Task<ServiceResponse<string>> UnsafeHandleAsync(GetCarDoorStatusQuery request,
            CancellationToken cancellationToken)
        {
            var car = Context.Cars.FirstOrDefault(r => r.DeviceId != null && r.DeviceId.Equals(request.DeviceId));

            if (car == null)
            {
                return ServiceResponseBuilder.Failure<string>(CarError.CarNotFound);
            }

            var doorStatus = car.IsDoorOpen ? DoorStatus.DoorOpen : DoorStatus.DoorClosed;
            return ServiceResponseBuilder.Success(doorStatus);
        }
    }
}