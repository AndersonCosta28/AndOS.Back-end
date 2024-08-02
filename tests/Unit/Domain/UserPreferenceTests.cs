namespace Unit.Domain;

public class UserPreferenceTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture) : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    [Fact]
    public void UpdateDefaultProgramToExtension_Should_Add_New_Default_Program()
    {
        // Arrange
        var extension = "txt";
        var program = "Notepad";
        var userPreference = userPreferenceFixture.CreateUserPreference(userFixture.DefaultUser);
        // Act
        userPreference.UpdateDefaultProgramToExtension(new List<DefaultProgramForExtension>
        {
            new DefaultProgramForExtension(extension, program)
        });

        // Assert
        Assert.Single(userPreference.DefaultProgramsToExtensions);
        Assert.Equal(program, userPreference.DefaultProgramsToExtensions[0].Program);
    }

    [Fact]
    public void RemoveDefaultProgramToExtension_Should_Remove_Existing_Default_Program()
    {
        // Arrange
        var extension = "txt";
        var userPreference = userPreferenceFixture.CreateUserPreference(userFixture.DefaultUser);

        userPreference.DefaultProgramsToExtensions.Add(new DefaultProgramForExtension(extension, "Notepad"));

        // Act
        userPreference.RemoveDefaultProgramToExtension(extension);

        // Assert
        Assert.Empty(userPreference.DefaultProgramsToExtensions);
    }

    [Fact]
    public void UpdateLanguage_Should_Update_Language()
    {
        // Arrange
        var newLanguage = "pt-BR";
        var userPreference = userPreferenceFixture.CreateUserPreference(userFixture.DefaultUser);

        // Act
        userPreference.UpdateLanguage(newLanguage);

        // Assert
        Assert.Equal(newLanguage, userPreference.Language);
    }
}
