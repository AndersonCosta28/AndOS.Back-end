using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Get.GetById;

namespace AndOS.Application.Folders.Get.GetById;

public class GetFolderByIdHandler(IReadRepository<Folder> folderRepository) : IRequestHandler<GetFolderByIdRequest, GetFolderByIdResponse>
{
    public async Task<GetFolderByIdResponse> Handle(GetFolderByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await folderRepository.ProjectToFirstOrDefaultAsync<GetFolderByIdResponse>(new GetFolderByIdSpec(request.Id), cancellationToken: cancellationToken);
        return result;
    }
}