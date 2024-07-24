namespace AndOS.API.Cors;

public static class CorsServiceCollection
{
    public static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration configuration)
    {
        //var corsSettings = configuration.GetSection("Cors");
        //var origins = corsSettings.GetSection("Origins").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("*",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });
        return services;
    }
}