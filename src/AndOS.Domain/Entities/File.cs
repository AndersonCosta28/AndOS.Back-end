using AndOS.Core.Enums;
using AndOS.Core.Schemas;
using AndOS.Domain.Exceptions;
using AndOS.Domain.Exceptions.FileExceptions;
using System.Text.RegularExpressions;

namespace AndOS.Domain.Entities;

public class File : SystemItem
{
    public File() { }

    public File(string name, string extension, Folder parentFolder, IUser user, string icon = "", string size = "") : base(name, user, parentFolder, icon)
    {
        UpdateSize(size);
        ValidateName(name);
        UpdateExtension(extension);
        UpdateParentFolder(parentFolder);
    }
    public string Extension { get; private set; }
    public string Size { get; private set; }
    public Folder ParentFolder { get; protected set; }
    public Guid ParentFolderId { get; set; }


    public override void UpdateName(string name)
    {
        ValidateName(name);
        if (name == Name)
            return;

        Name = name;
    }

    public void UpdateExtension(string extension)
    {
        ValidateExtension(extension);
        if (extension.Equals(Extension, StringComparison.OrdinalIgnoreCase))
            return;

        Extension = extension.ToLower();
    }

    public void UpdateSize(string size)
    {
        if (size == Size)
            return;

        Size = size;
    }

    public static bool ValidateName(string name)
    {
        if (name is { Length: < FileSchema.MinLenghtName or > FileSchema.MaxLenghtName })
            throw new InvalidFileNameLengthException();

        if (!Regex.IsMatch(name, FileSchema.RegexName))
            throw new InvalidFileNameCharacters();

        return true;
    }

    public static bool ValidateExtension(string extension)
    {
        if (extension is { Length: < FileSchema.MinLenghtExtension or > FileSchema.MaxLenghtExtension })
            throw new InvalidFileExtensionLengthException();

        if (!Regex.IsMatch(extension, FileSchema.RegexExtension))
            throw new InvalidFileExtensionCharacters();

        return true;
    }

    public void UpdateParentFolder(Folder parentFolder)
    {
        if (parentFolder.Type != FolderType.Storage && parentFolder.Type != FolderType.Common)
            throw new DomainLayerException("Files can only be added to storage or common folders");

        if (parentFolder == ParentFolder)
            return;

        parentFolder.AddFile(this);
        ParentFolder = parentFolder;
        ParentFolderId = parentFolder.Id;
    }

    public IUser GetUser()
    {
        if (ParentFolder.Type == FolderType.User)
            return ParentFolder.User;

        Folder currentFolder = ParentFolder;
        while (currentFolder is { Type: not FolderType.User })
            currentFolder = currentFolder.ParentFolder;

        return currentFolder?.User;
    }
}