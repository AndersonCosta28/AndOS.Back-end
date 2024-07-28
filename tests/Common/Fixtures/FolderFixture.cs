using AndOS.Domain.Entities;
using Bogus;

namespace Common.Fixtures;

public class FolderFixture
{
    public FolderFixture()
    {
        var faker = new Faker();
        var account = new AccountFixture().DefaultAccount;

        UserFolder = account.User.Folder;

        StorageFolder = account.Folder;

        CommonFolder = new Folder(faker.Random.String2(1, 100), account.User, StorageFolder);
        CommonFolder.UpdateType(FolderType.Common);
    }

    public Account Account => StorageFolder.Account;
    public Folder CommonFolder { get; private set; }
    public Folder StorageFolder { get; private set; }
    public IUser User => UserFolder.User;
    public Folder UserFolder { get; private set; }

    public Folder CreateNewCommonFolder()
    {
        var faker = new Faker();
        return new Folder(faker.Random.String2(1, 100), UserFolder.User, StorageFolder, type: FolderType.Common);
    }

    public Folder CreateNewStorageFolder()
    {
        var faker = new Faker();
        return new Folder(faker.Random.String2(1, 100), UserFolder.User, UserFolder, type: FolderType.Storage);
    }

    public List<Folder> GetFolders(int amount)
    {
        List<Folder> folders = [];
        for (int i = 0; i < amount; i++)
        {
            var folder = CreateNewCommonFolder();
            folders.Add(folder);
        }
        return folders;
    }
}