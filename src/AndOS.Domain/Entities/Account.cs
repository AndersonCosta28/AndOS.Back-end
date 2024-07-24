using AndOS.Common.Classes;
using AndOS.Core.Enums;
using AndOS.Core.StorageConfigs;
using System.Text.Json;

namespace AndOS.Domain.Entities;

public class Account : BaseAuditableEntity
{
    public Account() { }

    public Account(CloudStorage cloudStorageService)
    {
        CloudStorage = cloudStorageService;
    }

    public Account(string name, CloudStorage cloudStorage, IUser user, JsonDocument config)
    {
        Name = name;
        CloudStorage = cloudStorage;
        User = user;
        Config = config;
        Folder = new(name, user, user.Folder, type: FolderType.Storage);
        Folder.UpdateAccount(this);
    }

    public string Name { get; private set; }
    public CloudStorage CloudStorage { get; private set; }
    public IUser User { get; private set; }
    public Folder Folder { get; private set; }
    public Guid FolderId { get; set; }
    public JsonDocument Config { get; private set; }

    public void UpdateName(string name)
    {
        if (this.Name == name)
            return;
        Name = name;
    }

    public void UpdateCloudStorage(CloudStorage cloudStorage)
    {
        if (this.CloudStorage == cloudStorage)
            return;

        CloudStorage = cloudStorage;
    }

    public void UpdateUser(IUser user)
    {
        if (User?.Id == user.Id)
            return;

        User = user;
    }

    public void UpdateConfig(JsonDocument config)
    {
        Config = config;
    }
    public void UpdateConfig(AzureBlobStorageConfig newConfig)
    {
        var jsonString = JsonSerializer.Serialize(newConfig);
        Config = JsonDocument.Parse(jsonString);
    }

    //public void UpdateConfig(GcpStorageConfig newConfig)
    //{
    //    var jsonString = JsonSerializer.Serialize(newConfig);
    //    Config = JsonDocument.Parse(jsonString);
    //}

    //public void UpdateConfig(AwsS3Config newConfig)
    //{
    //    var jsonString = JsonSerializer.Serialize(newConfig);
    //    Config = JsonDocument.Parse(jsonString);
    //}

    public void UpdateFolder(Folder folder)
    {
        Folder = folder;
        FolderId = folder.Id;
    }
}