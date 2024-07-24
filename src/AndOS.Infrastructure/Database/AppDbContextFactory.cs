namespace AndOS.Infrastructure.Database;

public class AppDbContextFactory(IDbContextFactory<AppDbContext> pooledFactory, IServiceProvider services) : IDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext()
    {
        var appDbContext = pooledFactory.CreateDbContext();
        appDbContext.Database.EnsureCreated();
        var currentUserContext = services.GetRequiredService<ICurrentUserContext>();
        appDbContext.SetCurrentUser(currentUserContext);
        return appDbContext;
    }
}
