using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Address;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;

namespace AutoCab.Server.Features.Address;

public class GetAddressesQuery : IRequest<ServiceResponse<ICollection<AddressDto>>>
{
    public class GetAddressesQueryHandler : 
        ExtendedBaseHandler<GetAddressesQuery, ServiceResponse<ICollection<AddressDto>>>
    {
        public GetAddressesQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetAddressesQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<AddressDto>>> Handle(GetAddressesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get addresses error");
                return ServiceResponseBuilder.Failure<ICollection<AddressDto>>(AddressError.GetAddressesError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<AddressDto>>> UnsafeHandleAsync(
            GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var addresses = Context.Addresses;
            if (!addresses.Any())
            {
                return ServiceResponseBuilder.Failure<ICollection<AddressDto>>(AddressError.GetAddressesError);
            }
            var result = Mapper.Map<ICollection<AddressDto>>(addresses);
            return ServiceResponseBuilder.Success(result);
        }
    }
}