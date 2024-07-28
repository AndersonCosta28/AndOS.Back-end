using AndOS.Domain.Entities;
using Bogus;
namespace Common.Fixtures;

public class AccountFixture
{
    public AccountFixture()
    {
        var faker = new Faker();
        DefaultAccount = new Account(faker.Random.String2(4, 15), CloudStorage.Azure_BlobStorage, new UserFixture().DefaultUser, new ConfigFixture().AzureConfigDefault.ToJsonDocument());
    }

    public Account DefaultAccount { get; private set; }
    public Account CreateNewAccount(IUser user)
    {
        var faker = new Faker();
        return new Account(faker.Random.String2(4, 15), CloudStorage.Azure_BlobStorage, user, new ConfigFixture().AzureConfigDefault.ToJsonDocument());
    }

    public List<Account> GetAccounts(int amount)
    {
        var accounts = new List<Account>();
        var faker = new Faker();

        for (var i = 0; i < amount; i++)
            accounts.Add(new Account(faker.Random.String2(4, 15), CloudStorage.Azure_BlobStorage, new UserFixture().DefaultUser, new ConfigFixture().AzureConfigDefault.ToJsonDocument()));
        return accounts;
    }
}