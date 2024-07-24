using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Update.Rename;

namespace AndOS.Application.Folders.Update.Rename;

public class RenameFolderHandler(IRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer) : IRequestHandler<RenameFolderRequest>
{

    public async Task Handle(RenameFolderRequest request, CancellationToken cancellationToken)
    {
        Folder folder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(request.Id), cancellationToken) ?? throw new ApplicationLayerException(validationLocalizer["FolderNotFound"]);
        folder.UpdateName(request.Name);
        await folderRepository.UpdateAsync(folder, cancellationToken);
    }
}