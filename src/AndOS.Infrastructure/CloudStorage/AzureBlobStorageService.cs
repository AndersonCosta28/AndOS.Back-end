using AndOS.Core.StorageConfigs;
using AndOS.Shared.Consts;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.Text.Json;

namespace AndOS.Infrastructure.CloudStorage;

public class AzureBlobStorageService : ICloudStorageService
{
    //string azuriteAccountName = "devstoreaccount1";
    //string azuriteAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
    //string azuriteDefaultEndpointsProtocol = "http";
    //string azuriteUrl = "http://127.0.0.1:10000";
    //string azuriteBlobEndpoint => $"{azuriteUrl}/{azuriteAccountName}";
    //string azuriteConnectionString => $"DefaultEndpointsProtocol={azuriteDefaultEndpointsProtocol};AccountName={azuriteAccountName};AccountKey={azuriteAccountKey};BlobEndpoint={azuriteBlobEndpoint};";

    BlobServiceClient ServiceFactory(AzureBlobStorageConfig config)
    {
        string connectionString = config.ToString();
        BlobServiceClient blobServiceClient = new(connectionString);
        blobServiceClient.SetProperties(new BlobServiceProperties
        {
            // Define as regras CORS para o serviço de blob. Isso permite que aplicativos de diferentes origens acessem recursos no serviço de blob.
            Cors = [
             new()
             {
                 // Permite todos os domínios. Isso significa que qualquer domínio pode fazer solicitações ao serviço de blob.
                 AllowedOrigins = "*",
                 // Permite todos os métodos HTTP. Isso inclui GET, PUT, POST, DELETE e OPTIONS.
                 AllowedMethods = "GET, PUT, POST, DELETE, OPTIONS",
                 // Permite todos os cabeçalhos de solicitação. Isso significa que qualquer cabeçalho pode ser enviado nas solicitações.
                 AllowedHeaders = "*",
                 // Permite todos os cabeçalhos de resposta. Isso significa que qualquer cabeçalho pode ser enviado nas respostas.
                 ExposedHeaders = "*",
                 // Define o tempo máximo de cache para a resposta de pré-vôo OPTIONS em segundos. Isso controla quanto tempo um navegador pode armazenar em cache a resposta de uma solicitação OPTIONS.
                 MaxAgeInSeconds = 3600
             }
         ],
            // Especifica a versão da API do serviço de blob que o cliente deve usar ao fazer solicitações. Isso permite que você controle quais recursos e comportamentos estão disponíveis para o cliente.
            DefaultServiceVersion = AzureConsts.DefaultServiceVersion,
            // Configurações de log para o serviço de blob. Isso permite que você controle como as operações do serviço de blob são registradas.
            Logging = new()
            {
                Version = "1.0",
                // Indica se as operações de exclusão devem ser registradas.
                Delete = false,
                // Indica se as operações de leitura devem ser registradas.
                Read = true,
                // Indica se as operações de gravação devem ser registradas.
                Write = true,
                // Define a política de retenção para os logs. Isso controla por quanto tempo os logs são armazenados.
                RetentionPolicy = new()
                {
                    // Indica se a política de retenção está habilitada.
                    Enabled = false,
                    // Define o número de dias pelos quais os logs são armazenados. Se a política de retenção não estiver habilitada, este valor será ignorado.
                    Days = null
                }
            },
            // Configurações de métricas por hora para o serviço de blob. Isso permite que você controle como as métricas de uso do serviço de blob são registradas e armazenadas.
            HourMetrics = new()
            {
                Version = "1.0",
                // Indica se as métricas por hora estão habilitadas.
                Enabled = true,
                // Indica se as APIs do serviço de blob devem ser incluídas nas métricas.
                IncludeApis = true,
                // Define a política de retenção para as métricas por hora. Isso controla por quanto tempo as métricas são armazenadas.
                RetentionPolicy = new()
                {
                    // Indica se a política de retenção está habilitada.
                    Enabled = false,
                    // Define o número de dias pelos quais as métricas são armazenadas. Se a política de retenção não estiver habilitada, este valor será ignorado.
                    Days = null
                }
            },
            // Configurações de métricas por minuto para o serviço de blob. Isso permite que você controle como as métricas de uso do serviço de blob são registradas e armazenadas.
            MinuteMetrics = new()
            {
                Version = "1.0",
                // Indica se as métricas por minuto estão habilitadas.
                Enabled = true,
                // Indica se as APIs do serviço de blob devem ser incluídas nas métricas.
                IncludeApis = true,
                // Define a política de retenção para as métricas por minuto. Isso controla por quanto tempo as métricas são armazenadas.
                RetentionPolicy = new()
                {
                    // Indica se a política de retenção está habilitada.
                    Enabled = false,
                    // Define o número de dias pelos quais as métricas são armazenadas. Se a política de retenção não estiver habilitada, este valor será ignorado.
                    Days = null
                }
            }
        });

        return blobServiceClient;
    }

    public async Task<string> GetUploadUrlAsync(File file, Account account)
    {
        AzureBlobStorageConfig azureBlobStorageConfig = JsonSerializer.Deserialize<AzureBlobStorageConfig>(account.Config.RootElement.GetRawText());
        BlobServiceClient blobService = ServiceFactory(azureBlobStorageConfig);

        BlobContainerClient containerClient = blobService.GetBlobContainerClient(azureBlobStorageConfig.ContainerName);
        Azure.Response<BlobContainerInfo> containerInfo = await containerClient.CreateIfNotExistsAsync();
        //Azure.Response<bool> containerExists = await containerClient.ExistsAsync();
        BlobClient blobClient = containerClient.GetBlobClient(file.Id.ToString());
        var blobExists = await blobClient.ExistsAsync();
        if (!blobExists.Value)
            await blobClient.UploadAsync(BinaryData.FromString(string.Empty));
        //Azure.Response<bool> blobExists = blobClient.Exists();
        // Gera um link temporário para upload
        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(300)),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        StorageSharedKeyCredential storageSharedKeyCredential = new(azureBlobStorageConfig.AccountName, azureBlobStorageConfig.AccountKey);

        string sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

        // Retorna o link temporário para upload
        return $"{blobClient.Uri}?{sasToken}";
    }

    public async Task<string> GetUrlDownloadFileAsync(File file, Account account)
    {
        AzureBlobStorageConfig azureBlobStorageConfig = JsonSerializer.Deserialize<AzureBlobStorageConfig>(account.Config.RootElement.GetRawText());

        BlobServiceClient blobService = ServiceFactory(azureBlobStorageConfig);

        BlobContainerClient containerClient = blobService.GetBlobContainerClient(azureBlobStorageConfig.ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(file.Id.ToString());

        if (!await blobClient.ExistsAsync())
            throw new Exception("File not found.");


        // Gera um link temporário para o blob
        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        StorageSharedKeyCredential storageSharedKeyCredential = new(azureBlobStorageConfig.AccountName, azureBlobStorageConfig.AccountKey);
        string sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

        // Retorna o link temporário
        return $"{blobClient.Uri}?{sasToken}";
    }

    public async Task DeleteFileAsync(File file, Account account)
    {
        AzureBlobStorageConfig azureBlobStorageConfig = JsonSerializer.Deserialize<AzureBlobStorageConfig>(account.Config.RootElement.GetRawText());

        BlobServiceClient blobService = ServiceFactory(azureBlobStorageConfig);

        BlobContainerClient containerClient = blobService.GetBlobContainerClient(azureBlobStorageConfig.ContainerName);
        Azure.Response<BlobContainerInfo> containerInfo = await containerClient.CreateIfNotExistsAsync();
        BlobClient blobClient = containerClient.GetBlobClient(file.Id.ToString());
        await blobClient.DeleteIfExistsAsync();
    }
}