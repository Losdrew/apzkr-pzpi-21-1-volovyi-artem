using AutoCab.Db.DbContexts;
using AutoCab.Server.Features.Base;
using AutoCab.Shared.Dto.Account;
using AutoCab.Shared.Errors.ServiceErrors;
using AutoCab.Shared.ServiceResponseHandling;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoCab.Server.Features.Account;

public class GetUsersQuery : IRequest<ServiceResponse<ICollection<UserInfoDto>>>
{
    public class GetUsersQueryHandler :
        ExtendedBaseHandler<GetUsersQuery, ServiceResponse<ICollection<UserInfoDto>>>
    {
        public GetUsersQueryHandler(ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, ILogger<GetUsersQueryHandler> logger)
            : base(context, contextAccessor, mapper, logger)
        {
        }

        public override async Task<ServiceResponse<ICollection<UserInfoDto>>> Handle(GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await UnsafeHandleAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Get users error");
                return ServiceResponseBuilder.Failure<ICollection<UserInfoDto>>(UserError.GetUsersError);
            }
        }

        protected override async Task<ServiceResponse<ICollection<UserInfoDto>>> UnsafeHandleAsync(
            GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = Context.Users.Include(u => u.Role);
            var result = Mapper.Map<ICollection<UserInfoDto>>(users);
            return ServiceResponseBuilder.Success(result);
        }
    }
}
