namespace AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;

public record GetAccountFolderInParentFolderRequest(Guid ParentFolderId) : IRequest<Folder>;
