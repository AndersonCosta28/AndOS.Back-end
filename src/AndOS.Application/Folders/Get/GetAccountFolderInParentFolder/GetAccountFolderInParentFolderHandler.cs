using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;

namespace AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;

public class GetAccountFolderInParentFolderHandler(IReadRepository<Folder> folderRepository) : IRequestHandler<GetAccountFolderInParentFolderRequest, Folder>
{
    public async Task<Folder> Handle(GetAccountFolderInParentFolderRequest request, CancellationToken cancellationToken)
    {
        Folder folder = null;
        var folderId = request.ParentFolderId;
        do
        {
            folder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(folder?.ParentFolderId ?? request.ParentFolderId), cancellationToken);
            if (folder.Type == Core.Enums.FolderType.Storage)
                break;
        } while (folder != null || folder.Type != Core.Enums.FolderType.User || folder.ParentFolder != null || folder.ParentFolder?.Id != Guid.Empty);

        return folder;
    }
}
