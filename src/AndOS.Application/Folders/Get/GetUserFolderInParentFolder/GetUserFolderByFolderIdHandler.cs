using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;

namespace AndOS.Application.Folders.Get.GetUserFolderInParentFolder;

public class GetUserFolderByFolderIdHandler(IReadRepository<Folder> folderRepository) : IRequestHandler<GetUserFolderByFolderIdRequest, Folder>
{
    public async Task<Folder> Handle(GetUserFolderByFolderIdRequest request, CancellationToken cancellationToken)
    {
        Folder folder;
        do folder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(request.ParentFolderId), cancellationToken);
        while (folder == null || folder.Type == Core.Enums.FolderType.User || folder.ParentFolder == null || folder.ParentFolder.Id == Guid.Empty);
        return folder;
    }
}
