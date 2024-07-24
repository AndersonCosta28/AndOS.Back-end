using AndOS.Core.Enums;
using AndOS.Core.Schemas;
using AndOS.Domain.Exceptions;
using AndOS.Domain.Exceptions.FolderExceptions;
using System.Text.RegularExpressions;

namespace AndOS.Domain.Entities;
public class Folder : SystemItem
{
    public Guid? ParentFolderId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? AccountId { get; set; }
    public Guid OwnerId { get; set; }

    public virtual Folder ParentFolder { get; protected set; }
    public Account Account { get; private set; }
    public IUser User { get; private set; }

    public FolderType Type { get; private set; }

    public List<File> Files { get; } = [];
    public List<Folder> Folders { get; } = [];

    public Folder() { }

    public Folder(string name, IUser owner, Folder parentFolder = null, string icon = "", FolderType type = FolderType.Common) : base(name, owner, parentFolder, icon)
    {
        Type = type;
        ValidateName(name);
        UpdateParentFolder(parentFolder);
    }

    public override void UpdateName(string name)
    {
        ValidateName(name);
        if (name == Name)
            return;

        Name = name;
    }

    public static bool ValidateName(string name)
    {
        if (name is { Length: < FolderSchema.MinLenghtName or > FolderSchema.MaxLenghtName })
            throw new InvalidFolderNameLengthException();

        if (!Regex.IsMatch(name, FolderSchema.RegexName))
            throw new InvalidFolderNameCharacters();

        return true;
    }

    public void UpdateType(FolderType type)
    {
        var parentFolderHasSet = ParentFolder != null;
        if (type == FolderType.User && !parentFolderHasSet)
        {
            Type = type;
            return;
        }

        if (!parentFolderHasSet)
            throw new DomainLayerException("Parent folder cannot be null for type other than user");

        var parentFolderIsUserFolder = this.ParentFolder is { Type: FolderType.User };

        if (type == FolderType.Storage && !parentFolderIsUserFolder)
            throw new DomainLayerException("There can only be one storage folder directly below a user folder");

        Type = type;
    }

    public void UpdateAccount(Account account)
    {
        if (this.Type == FolderType.User)
            throw new DomainLayerException("You cannot set an account for a user folder");

        Account = account;
        AccountId = account?.Id;
    }

    public void UpdateUser(IUser user)
    {
        User = user;
        UserId = user?.Id;
    }

    public void UpdateParentFolder(Folder parentFolder)
    {
        if (parentFolder == ParentFolder)
            return;

        parentFolder.AddSubFolder(this);
        ParentFolder = parentFolder;
    }

    public void AddSubFolder(Folder folder)
    {
        var theFolderIsContainedInMyFolderList = this.Folders.Exists(x => x.Id == folder.Id);
        if (theFolderIsContainedInMyFolderList)
            return;

        if (this.Type == FolderType.User && folder.Type != FolderType.Storage)
            throw new DomainLayerException("Only storage folders can be added directly under a user folder");

        if (this.Type == FolderType.Storage && folder.Type != FolderType.Common)
            throw new DomainLayerException("Only common folders can be added under a storage folder");

        if (this.Type == FolderType.Common && folder.Type != FolderType.Common)
            throw new DomainLayerException("Only common folders can be added under a common folder");

        // if (this.Type != FolderType.User)
        //     folder.UpdateAccount(Account);
        // folder.UpdateUser(User);
        Folders.Add(folder);
    }

    public void AddFile(File file)
    {
        var theFileIsContainedInMyFileList = this.Files.Exists(x => x.Id == file.Id);
        if (theFileIsContainedInMyFileList)
            return;

        if (this.Type != FolderType.Storage && this.Type != FolderType.Common)
            throw new DomainLayerException("Files can only be added to storage or common folders");

        Files.Add(file);
    }

    public Account GetAccount()
    {
        if (this.Type == FolderType.Storage)
            return this.Account;

        Folder currentFolder = ParentFolder;
        while (currentFolder is { Type: not FolderType.Storage })
            currentFolder = currentFolder.ParentFolder;

        return currentFolder?.Account;
    }

    public IUser GetUser()
    {
        if (this.Type == FolderType.User)
            return this.User;

        Folder currentFolder = ParentFolder;
        while (currentFolder is { Type: not FolderType.User })
            currentFolder = currentFolder.ParentFolder;

        return currentFolder?.User;
    }
}