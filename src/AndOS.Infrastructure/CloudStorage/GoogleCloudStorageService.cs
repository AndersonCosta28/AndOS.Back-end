//using AndOS.Domain.Interfaces;
//using Google.Apis.Auth.OAuth2;
//using Google.Cloud.Storage.V1;

//namespace AndOS.Infrastructure.CloudStorage;

//public class GoogleCloudStorageService : ICloudStorageService
//{
//    private readonly StorageClient _storageClient;

//    public GoogleCloudStorageService(string projectId, string credentialsFilePath)
//    {
//        GoogleCredential credential = GoogleCredential.FromFile(credentialsFilePath).CreateScoped(new[] { "https://www.googleapis.com/auth/devstorage.read_write" });
//        _storageClient = StorageClient.Create(credential);
//    }

//    public Task<string> GetUploadUrlAsync(string bucketName, Guid fileId)
//    {
//        UrlSigner urlSigner = UrlSigner.FromCredential(GoogleCredential.FromFile("path/to/your/service-account-file.json"));
//        string url = urlSigner.Sign(bucketName, fileId.ToString(), TimeSpan.FromSeconds(30), HttpMethod.Put);

//        // Retorna o link temporário para upload
//        return Task.FromResult(url);
//    }

//    public async Task<string> GetUrlDownloadFileAsync(string bucketName, Guid fileId)
//    {
//        Google.Apis.Storage.v1.Data.Object objectMetadata = await _storageClient.GetObjectAsync(bucketName, fileId.ToString());
//        if (objectMetadata == null)
//            throw new Exception("File not found.");

//        UrlSigner urlSigner = UrlSigner.FromCredential(GoogleCredential.FromFile("path/to/your/service-account-file.json"));
//        string url = urlSigner.Sign(bucketName, fileId.ToString(), TimeSpan.FromHours(1), HttpMethod.Get);

//        // Retorna o link temporário
//        return url;
//    }

//    public async Task DeleteFileAsync(string bucketName, Guid fileId)
//    {
//        await _storageClient.DeleteObjectAsync(bucketName, fileId.ToString());
//    }
//}