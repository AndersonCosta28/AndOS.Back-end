using AndOS.Domain.Interfaces;
using AndOS.Infrastructure.Database;

namespace AndOS.Infrastructure.Repositories;

public class ReadRepository<T>(AppDbContext dbContext, IMapperService mapperService) : RepositoryBase<T>(dbContext, mapperService), IReadRepository<T> where T : class, IAggregateRoot
{
}