using Ardalis.Specification;

namespace AndOS.Application.Folders.Common.Specs;

public class GetFolderByParentFolderIsNullSpec : Specification<Folder>
{
    public GetFolderByParentFolderIsNullSpec()
    {
        Query
            .Where(x => x.ParentFolder == null);
    }
}