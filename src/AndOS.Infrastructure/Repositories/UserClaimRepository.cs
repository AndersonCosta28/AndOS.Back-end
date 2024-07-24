using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Identity.Entities;

namespace AndOS.Infrastructure.Repositories;

public class UserClaimRepository(AppDbContext context, IMapperService mapperService) : RepositoryBase<IUserClaim, ApplicationUserClaim>(context, mapperService)
{
}