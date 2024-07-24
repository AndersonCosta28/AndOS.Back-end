namespace AndOS.Infrastructure.CloudStorage;

public class CloudStorageServiceFactory(IServiceProvider serviceProvider) : ICloudStorageServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ICloudStorageService GetCloudStorageService(AndOS.Core.Enums.CloudStorage cloudStorageType)
    {
        return _serviceProvider.GetRequiredKeyedService<ICloudStorageService>(cloudStorageType);
    }
}
