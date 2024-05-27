using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Car;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Car;

public class GetCarQuery : IRequest<ServiceResponse<CarInfoDto>>
{
    public Guid CarId { get; set; }

    public class GetCarQueryHandler : 
        ExtendedBaseHandler<GetCarQuery, ServiceResponse<CarInfoDto>>
    {
        public GetCarQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetCarQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<CarInfoDto>> Handle(GetCarQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get car error");
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.GetCarsError);
            }
        }

        protected override async Task<ServiceResponse<CarInfoDto>> UnsafeHandleAsync(
            GetCarQuery request, CancellationToken cancellationToken)
        {
            var car = await Context.Cars.FindAsync(request.CarId);
            if (car == null)
            {
                return ServiceResponseBuilder.Failure<CarInfoDto>(CarError.GetCarsError);
            }
            var result = Mapper.Map<CarInfoDto>(car);
            return ServiceResponseBuilder.Success(result);
        }
    }
}