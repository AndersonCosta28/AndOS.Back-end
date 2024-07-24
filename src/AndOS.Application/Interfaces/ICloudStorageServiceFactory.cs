using AndOS.Core.Enums;

namespace AndOS.Application.Interfaces;

public interface ICloudStorageServiceFactory
{
    ICloudStorageService GetCloudStorageService(CloudStorage cloudStorageType);
}
