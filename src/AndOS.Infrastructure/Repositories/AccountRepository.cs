using AndOS.Infrastructure.Database;

namespace AndOS.Infrastructure.Repositories;

public class AccountRepository(AppDbContext dbContext, IMapperService mapperService) : RepositoryBase<Account>(dbContext, mapperService)
{
}