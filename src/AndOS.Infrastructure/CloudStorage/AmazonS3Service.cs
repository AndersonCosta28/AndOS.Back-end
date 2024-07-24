//using Amazon.S3;
//using Amazon.S3.Model;
//using AndOS.Domain.Interfaces;

//namespace AndOS.Infrastructure.CloudStorage;

//public class AmazonS3Service : ICloudStorageService
//{
//    private readonly AmazonS3Client _s3Client;
//    public AmazonS3Service(string accessKey, string secretKey, Amazon.RegionEndpoint region)
//    {
//        _s3Client = new AmazonS3Client(accessKey, secretKey, region);
//    }

//    public Task<string> GetUploadUrlAsync(string bucketName, Guid fileId)
//    {
//        GetPreSignedUrlRequest request = new()
//        {
//            BucketName = bucketName,
//            Key = fileId.ToString(),
//            Expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30)),
//            Verb = HttpVerb.PUT
//        };

//        string url = _s3Client.GetPreSignedURL(request);

//        // Retorna o link temporário para upload
//        return Task.FromResult(url);
//    }

//    public async Task<string> GetUrlDownloadFileAsync(string bucketName, Guid fileId)
//    {
//        await DoesS3ObjectExist(_s3Client, bucketName, fileId);

//        GetPreSignedUrlRequest request = new()
//        {
//            BucketName = bucketName,
//            Key = fileId.ToString(),
//            Expires = DateTime.UtcNow.AddHours(1)
//        };

//        string url = _s3Client.GetPreSignedURL(request);

//        // Retorna o link temporário
//        return url;
//    }

//    public async Task DeleteFileAsync(string bucketName, Guid fileId)
//    {
//        DeleteObjectRequest deleteObjectRequest = new()
//        {
//            BucketName = bucketName,
//            Key = fileId.ToString()
//        };
//        await _s3Client.DeleteObjectAsync(deleteObjectRequest);
//    }

//    private async Task DoesS3ObjectExist(AmazonS3Client client, string bucketName, Guid fileId)
//    {
//        try
//        {
//            GetObjectMetadataRequest request = new()
//            {
//                BucketName = bucketName,
//                Key = fileId.ToString()
//            };
//            await client.GetObjectMetadataAsync(request);
//        }
//        catch (AmazonS3Exception)
//        {
//            throw;
//        }
//    }
//}