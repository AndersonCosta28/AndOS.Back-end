using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Update.Rename;

namespace AndOS.Application.Files.Update.Rename;

public class RenameFileHandler(IRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer) : IRequestHandler<RenameFileRequest>
{
    public async Task Handle(RenameFileRequest request, CancellationToken cancellationToken)
    {
        var file = await fileRepository.FirstOrDefaultAsync(new GetFileByIdSpec(request.Id), cancellationToken) ??
                throw new ApplicationLayerException(validationLocalizer["FileNotFound"]);

        file.UpdateName(request.Name);
        file.UpdateExtension(request.Extension);
        await fileRepository.UpdateAsync(file, cancellationToken);
    }
}