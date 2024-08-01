namespace Common.Fixtures;
public class UserPreferenceFixture
{
    public UserPreferenceFixture()
    {

    }

    public UserPreference CreateUserPreference(IUser user)
    {
        var userPreference = new UserPreference(user);
        return userPreference;
    }

    public IEnumerable<UserPreference> GetUserPreferences(int amounts)
    {
        var items = new List<UserPreference>();
        for (int i = 0; i < amounts; i++)
        {
            var userFixture = new UserFixture();
            var userPreference = new UserPreference(userFixture.CreateNewUser());
            items.Add(userPreference);
        }
        return items;
    }
}
