using AndOS.Application.Folders.Common.Specs;
using AndOS.Application.Interfaces;
using AndOS.Core.Enums;
using AndOS.Domain.Interfaces;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;

namespace AndOS.Application.Folders.Get.GetByPath;

public class GetFolderByPathHandler(IReadRepository<Folder> folderRepository,
    ISender sender,
    IMapperService mapperService,
    IStringLocalizer<ValidationResource> validationLocalizer) : IRequestHandler<GetFolderByPathRequest, GetFolderByPathResponse>
{
    public async Task<GetFolderByPathResponse> Handle(GetFolderByPathRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Path))
        {
            var foldersInRoot = await folderRepository.ProjectToListAsync<ChildrenFolderDTO>(new GetFolderByParentFolderIsNullSpec(), cancellationToken);

            return new GetFolderByPathResponse()
            {
                Folders = foldersInRoot,
                CurrentPath = string.Empty,
                Files = [],
                FullPath = string.Empty,
                Name = string.Empty,
                ParentFolder = null,
                Icon = null,
                Permissions = [FolderPermission.Read]
            };
        }

        string[] folders = request.Path.TrimEnd('/').Split('/');
        Folder currentFolder = default;
        for (int i = 0; i < folders.Length; i++)
            currentFolder = await folderRepository.FirstOrDefaultAsync(new GetFolderByNameAndParentFolderIdSpec(folders[i], currentFolder?.Id), cancellationToken) ??
                throw new ApplicationLayerException(validationLocalizer["FolderNotFound"]);

        GetFolderByIdRequest requestFolderById = new GetFolderByIdRequest(currentFolder.Id);
        GetFolderByIdResponse result = await sender.Send(requestFolderById, cancellationToken);
        return await mapperService.MapAsync<GetFolderByPathResponse>(result);
    }
}