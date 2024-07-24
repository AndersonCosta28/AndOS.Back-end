using AndOS.Domain.Interfaces;

namespace AndOS.Infrastructure.Custom;

public static class CustomServiceCollection
{
    public static IServiceCollection AddCustomService(this IServiceCollection services) => services.AddScoped<ISystemItemService, SystemItemService>();
}
