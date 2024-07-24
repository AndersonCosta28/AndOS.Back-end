using AndOS.Core.Enums;
using AndOS.Core.StorageConfigs;
using System.Text.Json;

namespace Unit.Domain;

public class AccountTests
{
    private readonly IUser _user;
    private readonly AwsS3Config _awsS3Config;
    private readonly AzureBlobStorageConfig _azureBlobStorageConfig;
    private readonly GcpStorageConfig _gcpStorageConfig;

    public AccountTests()
    {
        _user = new ApplicationUser { Id = Guid.NewGuid() };
        _awsS3Config = new AwsS3Config
        {
            AccessKey = "accessKey",
            SecretKey = "secretKey",
            BucketName = "bucketName",
            Region = "region"
        };
        _azureBlobStorageConfig = new AzureBlobStorageConfig
        {
            AccountName = "accountName",
            AccountKey = "accountKey",
            ContainerName = "containerName",
            DefaultEndpointsProtocol = "https",
            EndpointSuffix = "https://example.com"
        };
        _gcpStorageConfig = new GcpStorageConfig
        {
            ProjectId = "projectId",
            BucketName = "bucketName",
            CredentialsJson = "{\"type\":\"service_account\"}"
        };
    }

    [Fact]
    /// <summary>
    /// Verifica se o construtor inicializa corretamente as propriedades da entidade Account.
    /// </summary>
    public void Constructor_ShouldInitializeProperties()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);

        Assert.Equal(CloudStorage.GCP_CloudStorage, account.CloudStorage);
        Assert.Null(account.Name);
        Assert.Null(account.User);
        Assert.Null(account.Config);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateName atualiza o nome corretamente.
    /// </summary>
    public void UpdateName_ShouldUpdateName()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);

        account.UpdateName("New Account Name");

        Assert.Equal("New Account Name", account.Name);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateCloudStorage atualiza o serviço de armazenamento em nuvem corretamente.
    /// </summary>
    public void UpdateCloudStorage_ShouldUpdateCloudStorage_WhenDifferent()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);

        account.UpdateCloudStorage(CloudStorage.AWS_S3Storage);

        Assert.Equal(CloudStorage.AWS_S3Storage, account.CloudStorage);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateCloudStorage não atualiza o serviço de armazenamento em nuvem quando o mesmo serviço é fornecido.
    /// </summary>
    public void UpdateCloudStorage_ShouldNotUpdateCloudStorage_WhenSame()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);

        account.UpdateCloudStorage(CloudStorage.GCP_CloudStorage);

        Assert.Equal(CloudStorage.GCP_CloudStorage, account.CloudStorage);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateUser atualiza o usuário corretamente.
    /// </summary>
    public void UpdateUser_ShouldUpdateUser_WhenDifferent()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);
        var newUser = new ApplicationUser { Id = Guid.NewGuid() };

        account.UpdateUser(newUser);

        Assert.Equal(newUser, account.User);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateUser não atualiza o usuário quando o mesmo usuário é fornecido.
    /// </summary>
    public void UpdateUser_ShouldNotUpdateUser_WhenSame()
    {
        var account = new Account(CloudStorage.GCP_CloudStorage);
        account.UpdateUser(_user);

        account.UpdateUser(_user);

        Assert.Equal(_user, account.User);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateConfig atualiza a configuração corretamente com AwsS3Config.
    /// </summary>
    public void UpdateConfig_ShouldUpdateConfig_WithAwsS3Config()
    {
        Account account = new Account(CloudStorage.GCP_CloudStorage);
        JsonDocument configJson = JsonSerializer.SerializeToDocument(_awsS3Config);

        account.UpdateConfig(configJson);

        Assert.Equal(configJson, account.Config);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateConfig atualiza a configuração corretamente com AzureBlobStorageConfig.
    /// </summary>
    public void UpdateConfig_ShouldUpdateConfig_WithAzureBlobStorageConfig()
    {
        Account account = new Account(CloudStorage.GCP_CloudStorage);
        JsonDocument configJson = JsonSerializer.SerializeToDocument(_azureBlobStorageConfig);

        account.UpdateConfig(configJson);

        Assert.Equal(configJson, account.Config);
    }

    [Fact]
    /// <summary>
    /// Testa se o método UpdateConfig atualiza a configuração corretamente com GcpStorageConfig.
    /// </summary>
    public void UpdateConfig_ShouldUpdateConfig_WithGcpStorageConfig()
    {
        Account account = new Account(CloudStorage.GCP_CloudStorage);
        JsonDocument configJson = JsonSerializer.SerializeToDocument(_gcpStorageConfig);

        account.UpdateConfig(configJson);

        Assert.Equal(configJson, account.Config);
    }

    [Fact]
    public void UpdateFolder_ShouldUpdateFolder()
    {
        // Arrange
        var account = new Account(CloudStorage.GCP_CloudStorage);
        var folder = new Folder("testFolder", _user);

        // Act
        account.UpdateFolder(folder);

        // Assert
        Assert.Equal(folder, account.Folder);
    }
}