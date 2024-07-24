using AndOS.Domain.Exceptions.UserExceptions;

namespace AndOS.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>, IUser
{
    public ApplicationUser()
    {
    }

    public ApplicationUser(string userName) : base(userName)
    {
        UpdateUserName(userName);
        Folder = new Folder(userName, this);
        Folder.UpdateType(FolderType.User);
        Folder.UpdateUser(this);
        Folder.Id = Guid.NewGuid();
    }

    public List<Account> Accounts { get; set; }
    public Folder Folder { get; set; }
    public Guid FolderId { get; set; }
    public bool UserNameUpdated { get; set; }
    public void UpdateUserName(string userName)
    {
        if (UserNameUpdated)
            throw new UserNameAlreadyUpdatedException();

        IUser.ValidateUserName(userName);
        UserName = userName;
        UserNameUpdated = true;
    }
}