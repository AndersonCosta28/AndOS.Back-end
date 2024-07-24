namespace AndOS.Infrastructure.Database;

public static class DatabaseServiceCollection
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("postgresdb");

        services.AddPooledDbContextFactory<AppDbContext>((dbContextOptionsBuilder) =>
            dbContextOptionsBuilder.UseNpgsql(connectionString, op => op.EnableRetryOnFailure())
        );
        services.AddScoped<AppDbContextFactory>();
        services.AddScoped(sp => sp.GetRequiredService<AppDbContextFactory>().CreateDbContext());
        return services;
    }
}
