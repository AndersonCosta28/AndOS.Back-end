using AndOS.Infrastructure.Database;

namespace AndOS.Infrastructure.Repositories;

public class FileRepository(AppDbContext dbContext, IMapperService mapperService) : RepositoryBase<File>(dbContext, mapperService)
{
}