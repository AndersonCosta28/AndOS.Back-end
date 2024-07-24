using AndOS.Infrastructure.Database;
using AndOS.Infrastructure.Identity.Entities;

namespace AndOS.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<IUser, ApplicationUser>
{
    public UserRepository(AppDbContext context, IMapperService mapperService) : base(context, mapperService)
    {
    }
}