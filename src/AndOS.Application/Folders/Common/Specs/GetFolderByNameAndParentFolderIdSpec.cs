using Ardalis.Specification;

namespace AndOS.Application.Folders.Common.Specs;

public class GetFolderByNameAndParentFolderIdSpec : Specification<Folder>
{
    public GetFolderByNameAndParentFolderIdSpec(string name, Guid? parentFolderId)
    {
        Query
            .Where(x => parentFolderId != null ? x.ParentFolder != null && x.ParentFolder.Id == parentFolderId
                                                             : x.ParentFolder == null)
            .Where(x => x.Name.ToLower() == name.ToLower());
    }
}