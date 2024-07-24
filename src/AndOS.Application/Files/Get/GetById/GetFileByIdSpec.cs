using Ardalis.Specification;

namespace AndOS.Application.Files.Get.GetById;

public class GetFileByIdSpec : Specification<File>
{
    public GetFileByIdSpec(Guid id)
    {
        Query
            .Include(x => x.ParentFolder)
                .ThenInclude(x => x.ParentFolder)
            .Where(x => x.Id == id);
    }
}