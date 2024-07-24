using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Identity.Entities;

namespace AndOS.Infrastructure.Repositories;

public class RoleClaimRepository(AppDbContext context, IMapperService mapperService)
        : RepositoryBase<IRoleClaim, ApplicationRoleClaim>(context, mapperService)
{
}