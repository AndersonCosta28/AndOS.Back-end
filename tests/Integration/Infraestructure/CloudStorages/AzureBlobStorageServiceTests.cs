using AndOS.Core.StorageConfigs;
using AndOS.Domain.Entities;
using AndOS.Infrastructure.CloudStorage;
using Azure;
using Azure.Storage.Blobs;
using Common.Fixtures;
using System.Text.Json;
namespace Integration.Infraestructure.CloudStorages;

public class AzureBlobStorageServiceTests(FileFixture fileFixture, ConfigFixture configFixture) : IClassFixture<FileFixture>, IClassFixture<ConfigFixture>
{
    private readonly AzureBlobStorageService _azureBlobStorageService = new();
    private readonly ConfigFixture _configFixture = configFixture;
    AzureBlobStorageConfig _azureConfig = configFixture.AzureConfigDefault;
    [Fact]
    public async Task GetUploadUrlAsync_ShouldReturnUploadUrl()
    {
        var file = fileFixture.DefaultFile;
        var account = file.ParentFolder.GetAccount();

        var uploadUrl = await _azureBlobStorageService.GetUploadUrlAsync(file, account);

        Assert.NotNull(uploadUrl);
        Assert.Contains(_azureConfig.Url, uploadUrl);
    }

    [Fact]
    public async Task GetUrlDownloadFileAsync_ShouldReturnDownloadUrl()
    {
        var file = fileFixture.DefaultFile;
        var account = file.ParentFolder.GetAccount();
        await CreateFile(file, account);

        // Chamar o método a ser testado
        var downloadUrl = await _azureBlobStorageService.GetUrlDownloadFileAsync(file, account);

        // Verificar o resultado
        Assert.NotNull(downloadUrl);
        Assert.Contains(_azureConfig.Url, downloadUrl);
    }

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile()
    {
        var file = fileFixture.DefaultFile;
        var account = file.ParentFolder.GetAccount();

        BlobClient blobClient = await CreateFile(file, account);

        // Chamar o método a ser testado
        await _azureBlobStorageService.DeleteFileAsync(file, account);

        // Verificar se o método DeleteAsync foi chamado
        var exists = await blobClient.ExistsAsync();
        Assert.False(exists.Value);
    }


    [Fact]
    public async Task GetUrlDownloadFileAsync_ShouldThrowException_WhenFileNotFound()
    {
        var file = fileFixture.DefaultFile;
        var account = file.ParentFolder.GetAccount();

        // Chamar o método a ser testado e verificar a exceção
        var exception = await Assert.ThrowsAsync<Exception>(() => _azureBlobStorageService.GetUrlDownloadFileAsync(file, account));
        Assert.Contains("File not found.", exception.Message);
    }

    //[Fact]
    //public async Task DeleteFileAsync_ShouldThrowException_WhenFileNotFound()
    //{
    //    // Configurar mocks
    //    var file = fileFixture.DefaultFile;
    //    var account = file.ParentFolder.GetAccount();

    //    // Chamar o método a ser testado e verificar a exceção
    //    var exception = await Assert.ThrowsAsync<RequestFailedException>(() => _azureBlobStorageService.DeleteFileAsync(file, account));
    //    Assert.Contains("The specified blob does not exist.", exception.Message);
    //}

    private static async Task<BlobClient> CreateFile(AndOS.Domain.Entities.File file, Account account)
    {
        var storageConfig = account.Config.RootElement.Deserialize<AzureBlobStorageConfig>();
        var connectionString = storageConfig.ToString();
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.ContainerName);
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.UploadBlobAsync(file.Id.ToString(), new BinaryData("This is a test file"));
        var blobClient = containerClient.GetBlobClient(file.Id.ToString());

        return blobClient;
    }
}