namespace AndOS.Application.Folders.Get.GetUserFolderInParentFolder;

public record GetUserFolderByFolderIdRequest(Guid Id, Guid ParentFolderId) : IRequest<Folder>;
