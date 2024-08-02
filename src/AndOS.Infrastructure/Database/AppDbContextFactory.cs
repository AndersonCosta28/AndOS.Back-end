using Microsoft.EntityFrameworkCore.Design;

namespace AndOS.Infrastructure.Database;

public class AppDbContextFactory : IDbContextFactory<AppDbContext>,
    IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly IDbContextFactory<AppDbContext> _pooledFactory;
    private readonly IServiceProvider _services;

    public AppDbContextFactory(IDbContextFactory<AppDbContext> pooledFactory, IServiceProvider services)
    {
        _pooledFactory = pooledFactory;
        _services = services;
    }

    public AppDbContextFactory() { }

    public AppDbContext CreateDbContext()
    {
        var appDbContext = _pooledFactory.CreateDbContext();
        appDbContext.Database.Migrate();
        var currentUserContext = _services.GetRequiredService<ICurrentUserContext>();
        appDbContext.SetCurrentUser(currentUserContext);
        return appDbContext;
    }

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=1234;Database=andos");

        return new AppDbContext(optionsBuilder.Options);
    }
}
