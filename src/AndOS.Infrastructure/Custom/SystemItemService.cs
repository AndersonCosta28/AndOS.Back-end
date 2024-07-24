using AndOS.Application.Folders.Get.GetById;
using AndOS.Domain.Interfaces;

namespace AndOS.Infrastructure.Custom;

public class SystemItemService(IReadRepository<Folder> folderRepository) : ISystemItemService
{
    public async Task<string> GetCurrentPath(Folder folder) =>
        await internalGetCurrentPath(folder, true);

    public async Task<string> GetCurrentPath(File file)
    {
        string pathToFolder = await GetCurrentPath(file.ParentFolder);
        return pathToFolder;
    }

    async Task<string> internalGetCurrentPath(Folder folder, bool isFirst)
    {
        Folder currentFolder = await folderRepository.FirstOrDefaultAsync(new GetFolderByIdSpec(folder.Id)) ??
                                        throw new Exception($"Folder with Id {folder} doesn't exists");
        if (currentFolder.ParentFolder == null)
            return currentFolder.Name; // Este é o folder raiz, então retorna apenas o nome        

        string parentPath = await internalGetCurrentPath(currentFolder.ParentFolder, false);
        // Aqui, retornamos apenas o caminho do pai, sem adicionar o nome da pasta atual.
        return parentPath != null ? isFirst ? $"{parentPath}" : $"{parentPath}/{currentFolder.Name}" : string.Empty;
    }
}
