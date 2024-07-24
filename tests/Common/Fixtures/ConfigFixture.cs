using AndOS.Core.StorageConfigs;

namespace Common.Fixtures;

public class ConfigFixture
{
    public AzureBlobStorageConfig AzureConfigDefault { get; set; } = new AzureBlobStorageConfig()
    {
        AccountName = "devstoreaccount1",
        AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==",
        ContainerName = "testcontainer",
        DefaultEndpointsProtocol = "http",
        EndpointSuffix = "127.0.0.1:10000",
        IsAzurite = true,
    };
}