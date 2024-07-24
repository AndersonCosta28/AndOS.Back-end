namespace AndOS.Infrastructure.Repositories;

public static class RepositoryServiceCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<File>, FileRepository>();
        services.AddScoped<IReadRepository<File>, FileRepository>();

        services.AddScoped<IRepository<Folder>, FolderRepository>();
        services.AddScoped<IReadRepository<Folder>, FolderRepository>();

        services.AddScoped<IRepository<Account>, AccountRepository>();
        services.AddScoped<IReadRepository<Account>, AccountRepository>();

        services.AddScoped<IRepository<IUser>, UserRepository>();
        services.AddScoped<IReadRepository<IUser>, UserRepository>();

        services.AddScoped<IRepository<IUserClaim>, UserClaimRepository>();
        services.AddScoped<IReadRepository<IUserClaim>, UserClaimRepository>();

        services.AddScoped<IRepository<IRoleClaim>, RoleClaimRepository>();
        services.AddScoped<IReadRepository<IRoleClaim>, RoleClaimRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}