using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Delete;
namespace AndOS.Application.Folders.Delete;

public class DeleteFolderHandler(IRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer) : IRequestHandler<DeleteFolderRequest>
{
    private readonly IRepository<Folder> _folderRepository = folderRepository;
    private readonly IStringLocalizer<ValidationResource> _validationLocalizer = validationLocalizer;

    public async Task Handle(DeleteFolderRequest request, CancellationToken cancellationToken)
    {
        Folder folder = await _folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(request.Id), cancellationToken) ??
                        throw new ApplicationLayerException(_validationLocalizer["FolderNotFound"]);
        await _folderRepository.DeleteAsync(folder, cancellationToken);
    }
}