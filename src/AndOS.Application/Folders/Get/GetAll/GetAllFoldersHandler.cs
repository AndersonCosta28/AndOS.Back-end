using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Get.GetAll;

namespace AndOS.Application.Folders.Get.GetAll;

internal class GetAllFoldersHandler(IReadRepository<Folder> folderRepository, IMapperService mapper) : IRequestHandler<GetAllFoldersRequest, IEnumerable<FolderDTO>>
{
    private readonly IReadRepository<Folder> _folderRepository = folderRepository;
    private readonly IMapperService _mapper = mapper;

    public async Task<IEnumerable<FolderDTO>> Handle(GetAllFoldersRequest request, CancellationToken cancellationToken)
    {
        var result = await _folderRepository.ProjectToListAsync<FolderDTO>(cancellationToken);
        return result;
    }
}