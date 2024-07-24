namespace AndOS.Application.Interfaces;

public interface ICloudStorageService
{
    Task<string> GetUploadUrlAsync(File file, Account account);
    Task<string> GetUrlDownloadFileAsync(File file, Account account);
    Task DeleteFileAsync(File file, Account account);
}