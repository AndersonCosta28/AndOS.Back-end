using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Core.Constants;
using AndOS.Core.Enums;
using AndOS.Domain.Interfaces;
using AndOS.Shared.Requests.Files.Create;

namespace AndOS.Application.Files.Create;

public class CreateFileHandler(
    IRepository<File> fileRepository,
    IReadRepository<Folder> folderRepository,
    ICurrentUserContext currentUserContext,
    IStringLocalizer<ValidationResource> validationLocalizer,
    ICloudStorageServiceFactory cloudStorageServiceFactory,
    IAuthorizationService authorizationService,
    ISender sender) : IRequestHandler<CreateFileRequest, CreateFileResponse>
{
    public async Task<CreateFileResponse> Handle(CreateFileRequest request, CancellationToken cancellationToken)
    {
        var user = await currentUserContext.GetCurrentUserAsync(cancellationToken);
        var parentFolder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(request.ParentFolderId), cancellationToken) ??
                                  throw new ApplicationLayerException(validationLocalizer["ParentFolderNotFound"]);

        var accountFolder = await sender.Send(new GetAccountFolderInParentFolderRequest(parentFolder.Id), cancellationToken);

        var account = accountFolder.Account;

        var file = new File(request.Name, request.Extension, parentFolder, user);

        await fileRepository.AddAsync(file, cancellationToken);
        var claimsToAdd = new List<IUserClaim>()
             {
                authorizationService.CreateUserClaim(file.Id, FilePermission.Read, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(file.Id, FilePermission.Write, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(file.Id, FilePermission.Rename, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(file.Id, FilePermission.Delete, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(file.Id, FilePermission.Shared, ClaimConsts.VALUE_TRUE),
             };

        foreach (var claim in claimsToAdd)
            await authorizationService.UpdatePermissionAsync(user, claim, cancellationToken);
        var cloudStorageService = cloudStorageServiceFactory.GetCloudStorageService(account.CloudStorage);
        var url = await cloudStorageService.GetUploadUrlAsync(file, account);


        return new CreateFileResponse(file.Id, url, account.CloudStorage);
    }
}