using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Delete;
namespace AndOS.Application.Files.Delete;

public class DeleteFileHandler(IRepository<File> fileRepository,
    IStringLocalizer<ValidationResource> validationLocalizer,
    ICloudStorageServiceFactory cloudStorageServiceFactory,
    ISender sender) : IRequestHandler<DeleteFileRequest>
{
    public async Task Handle(DeleteFileRequest request, CancellationToken cancellationToken)
    {
        var file = await fileRepository.FirstOrDefaultAsync(new GetFileByIdSpec(request.Id), cancellationToken) ??
                        throw new ApplicationLayerException(validationLocalizer["FileNotFound"]);

        var accountFolder = await sender.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), cancellationToken);

        var account = accountFolder.Account;

        await fileRepository.DeleteAsync(file, cancellationToken);

        var cloudStorageService = cloudStorageServiceFactory.GetCloudStorageService(account.CloudStorage);
        await cloudStorageService.DeleteFileAsync(file, account);
    }
}