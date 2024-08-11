using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Update.Content;

namespace AndOS.Application.Files.Update.Content;

public class UploadFileHandler(
    ISender sender,
    IRepository<File> fileRepository,
    IStringLocalizer<ValidationResource> validationLocalizer,
    ICloudStorageServiceFactory cloudStorageServiceFactory) : IRequestHandler<UpdateContentFileRequest, UpdateContentFileResponse>
{
    public async Task<UpdateContentFileResponse> Handle(UpdateContentFileRequest request, CancellationToken cancellationToken)
    {
        var file = await fileRepository.FirstOrDefaultAsync(new GetFileByIdSpec(request.Id), cancellationToken) ??
                        throw new ApplicationLayerException(validationLocalizer["FileNotFound"]);

        var accountFolder = await sender.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), cancellationToken);

        var account = accountFolder.Account;

        await fileRepository.UpdateAsync(file, cancellationToken);
        ICloudStorageService cloudStorageService = cloudStorageServiceFactory.GetCloudStorageService(account.CloudStorage);
        var url = await cloudStorageService.GetUploadUrlAsync(file, account);
        return new UpdateContentFileResponse(url, account.CloudStorage);
    }
}