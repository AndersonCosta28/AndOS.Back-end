using AndOS.Infrastructure.Database;

namespace AndOS.Infrastructure.Repositories;

public class FolderRepository(AppDbContext dbContext, IMapperService mapperService) : RepositoryBase<Folder>(dbContext, mapperService)
{
}