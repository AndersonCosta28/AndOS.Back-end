using AndOS.Infrastructure.Database;

namespace AndOS.Infrastructure.Repositories;
public class UserPreferenceRepository(AppDbContext appDbContext, IMapperService mapperService)
    : RepositoryBase<UserPreference>(appDbContext, mapperService);
