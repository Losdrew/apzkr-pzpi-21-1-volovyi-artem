using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class GetCarsQuery : IRequest<ServiceResponse<ICollection<CarInfoDto>>>
{
    public class GetCarsQueryHandler : 
        ExtendedBaseHandler<GetCarsQuery, ServiceResponse<ICollection<CarInfoDto>>>
    {
        public GetCarsQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetCarsQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<CarInfoDto>>> Handle(GetCarsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get cars error");
                return ServiceResponseBuilder.Failure<ICollection<CarInfoDto>>(CarError.GetCarsError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<CarInfoDto>>> UnsafeHandleAsync(
            GetCarsQuery request, CancellationToken cancellationToken)
        {
            var cars = Context.Cars;
            if (!cars.Any())
            {
                return ServiceResponseBuilder.Failure<ICollection<CarInfoDto>>(CarError.GetCarsError);
            }
            var result = Mapper.Map<ICollection<CarInfoDto>>(cars);
            return ServiceResponseBuilder.Success(result);
        }
    }
}