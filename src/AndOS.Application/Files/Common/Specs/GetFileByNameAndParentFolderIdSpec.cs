using Ardalis.Specification;

namespace AndOS.Application.Files.Common.Specs;

public class GetFileByNameAndParentFolderIdSpec : Specification<File>
{
    public GetFileByNameAndParentFolderIdSpec(string name, string extension, Guid parentFolderId)
    {
        Query
            .Where(x => x.ParentFolder.Id == parentFolderId)
            .Where(x => x.Name.ToLower().Trim() == name.ToLower().Trim() && x.Extension == extension);
    }
}