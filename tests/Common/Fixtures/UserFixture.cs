using AndOS.Infrastructure.Identity.Entities;
using Bogus;

namespace Common.Fixtures;

public class UserFixture
{
    public UserFixture()
    {
        var faker = new Faker();

        DefaultUser = new ApplicationUser(faker.Random.String2(4, 15))
        {
            Email = faker.Person.Email,
            Id = Guid.NewGuid()
        };
    }

    public ApplicationUser DefaultUser { get; private set; }

    public List<ApplicationUser> GetUsers(int amount)
    {
        var users = new List<ApplicationUser>();
        for (int i = 0; i < amount; i++)
        {
            var faker = new Faker();
            users.Add(new ApplicationUser(faker.Random.String2(4, 15))
            {
                Email = faker.Person.Email
            });
        }

        return users;
    }

    public ApplicationUser CreateNewUser()
    {
        var faker = new Faker();

        var user = new ApplicationUser(faker.Random.String2(4, 15))
        {
            Email = faker.Person.Email,
            Id = Guid.NewGuid()
        };

        return user;
    }
}