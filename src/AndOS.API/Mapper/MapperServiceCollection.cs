using Mapster;
using MapsterMapper;
using System.Reflection;
namespace AndOS.API.Mapper;

public static class MapperServiceCollection
{
    public static IServiceCollection AddMapperService(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapperService, MapperService>();
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}