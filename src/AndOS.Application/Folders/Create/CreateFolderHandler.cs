using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Core.Constants;
using AndOS.Core.Enums;
using AndOS.Domain.Interfaces;
using AndOS.Shared.Requests.Folders.Create;
namespace AndOS.Application.Folders.Create;

public class CreateFolderHandler(IUnitOfWork unitOfWork,
    IRepository<Folder> folderRepository,
    ICurrentUserContext currentUserContext,
    IAuthorizationService authorizationService,
    IStringLocalizer<ValidationResource> validationLocalizer) : IRequestHandler<CreateFolderRequest, CreateFolderResponse>
{
    public async Task<CreateFolderResponse> Handle(CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var operation = async (CancellationToken ct) =>
        {
            IUser user = await currentUserContext.GetCurrentUserAsync(ct);

            var folder = new Folder(request.Name, user, null);

            Folder parentFolder = null;
            if (request.ParentFolderId is Guid parentFolderId)
            {
                parentFolder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(parentFolderId), ct) ??
                               throw new ApplicationLayerException(validationLocalizer["ParentFolderNotFound"]);
                folder.UpdateParentFolder(parentFolder);
            }

            await folderRepository.AddAsync(folder, ct);

            var claimsFromParentFolder =
                 parentFolder == null ? [] :
                      (await authorizationService.GetFolderUserClaims(parentFolder.Id))
                     .Where(x => x.UserId != user.Id)
                     .ToList();

            List<IUserClaim> claimsToAdd =
            [
                authorizationService.CreateUserClaim(folder.Id, FolderPermission.Read, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(folder.Id, FolderPermission.Write, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(folder.Id, FolderPermission.Rename, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(folder.Id, FolderPermission.Delete, ClaimConsts.VALUE_TRUE),
                authorizationService.CreateUserClaim(folder.Id, FolderPermission.Shared, ClaimConsts.VALUE_TRUE),
            ];

            List<IUserClaim> newClaims = [.. claimsFromParentFolder, .. claimsToAdd];
            foreach (var claim in newClaims)
                await authorizationService.UpdatePermissionAsync(user, claim, ct);

            return new CreateFolderResponse(folder.Id);
        };
        var result = await unitOfWork.ExecuteAsync(operation, cancellationToken);
        return result;
    }
}