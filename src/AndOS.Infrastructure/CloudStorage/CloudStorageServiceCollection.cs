namespace AndOS.Infrastructure.CloudStorage;

internal static class CloudStorageServiceCollection
{
    internal static IServiceCollection AddCloudStorageService(this IServiceCollection services)
    {
        services.AddKeyedScoped<ICloudStorageService, AzureBlobStorageService>(Core.Enums.CloudStorage.Azure_BlobStorage);
        //services.AddKeyedScoped<ICloudStorageService, AmazonS3Service>(CloudStorageService.AWS_S3Storage);
        //services.AddKeyedScoped<ICloudStorageService, GoogleCloudStorageService>(CloudStorageService.GCP_CloudStorage);
        services.AddScoped<ICloudStorageServiceFactory, CloudStorageServiceFactory>();
        return services;
    }
}