using Bogus;
using File = AndOS.Domain.Entities.File;
namespace Common.Fixtures;

public class FileFixture
{
    public File DefaultFile { get; private set; }

    public FileFixture()
    {
        var faker = new Faker();
        var folderFixture = new FolderFixture();
        DefaultFile = new File(faker.Random.String2(1, 100), faker.Random.String2(1, 3), folderFixture.CommonFolder, folderFixture.UserFolder.User, "icon", "1KB")
        {
            Id = Guid.NewGuid()
        };
    }

    public List<File> GetFiles(int amount)
    {
        var folderFixture = new FolderFixture();

        List<File> files = [];
        var faker = new Faker();

        for (int i = 0; i < amount; i++)
        {
            var file = new File(faker.Random.String2(1, 100), faker.Random.String2(1, 3), folderFixture.CommonFolder, folderFixture.UserFolder.User, "icon", "1KB")
            {
                Id = Guid.NewGuid()
            };
            files.Add(file);
        }

        return files;
    }
}