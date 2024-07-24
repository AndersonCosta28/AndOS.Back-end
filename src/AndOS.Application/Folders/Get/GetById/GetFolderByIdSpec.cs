using Ardalis.Specification;

namespace AndOS.Application.Folders.Get.GetById;

public class GetFolderByIdSpec : Specification<Folder>
{
    public GetFolderByIdSpec(Guid id)
    {
        Query
            .Include(x => x.ParentFolder)
                .ThenInclude(x => x.Account)
            .Include(x => x.Folders)
            .Include(x => x.Files)
            .Include(x => x.Account)
            .Include(x => x.User)
            .Where(x => x.Id == id);
    }
}