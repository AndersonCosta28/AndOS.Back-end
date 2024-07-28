using AndOS.Domain.Exceptions.UserExceptions;

namespace Unit.Domain;

public class UserTests
{
    [Fact]
    public void UpdateUserName_ShouldUpdateUserName_WhenUserNameIsValid()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser();
        string validUserName = "Valid123";

        // Act
        user.UpdateUserName(validUserName);

        // Assert
        Assert.Equal(validUserName, user.UserName);
        Assert.True(user.UserNameUpdated);
    }

    [Fact]
    public void UpdateUserName_ShouldThrowException_WhenUserNameAlreadyUpdated()
    {
        // Arrange
        ApplicationUser user = new ApplicationUser();
        string firstUserName = "First123";
        string secondUserName = "Second123";
        user.UpdateUserName(firstUserName);

        // Act & Assert
        UserNameAlreadyUpdatedException exception = Assert.Throws<UserNameAlreadyUpdatedException>(() => user.UpdateUserName(secondUserName));
    }

    [Fact]
    public void ValidateUserName_ShouldThrowException_WhenUserNameIsTooShort()
    {
        // Arrange
        string shortUserName = "Ab1"; // Menor que 4 caracteres

        // Act & Assert
        InvalidUserNameLengthException exception = Assert.Throws<InvalidUserNameLengthException>(() => IUser.ValidateUserName(shortUserName));
    }

    [Fact]
    public void ValidateUserName_ShouldThrowException_WhenUserNameIsTooLong()
    {
        // Arrange
        string longUserName = new string('A', UserSchema.MaxLenghtUserName + 1); // Maior que 15 caracteres

        // Act & Assert
        InvalidUserNameLengthException exception = Assert.Throws<InvalidUserNameLengthException>(() => IUser.ValidateUserName(longUserName));
    }

    [Fact]
    public void ValidateUserName_ShouldThrowException_WhenUserNameContainsInvalidCharacters()
    {
        // Arrange
        string invalidUserName = "Invalid@Name"; // Contém '@', que não é permitido

        // Act & Assert
        InvalidUserNameCharacters exception = Assert.Throws<InvalidUserNameCharacters>(() => IUser.ValidateUserName(invalidUserName));
    }

    [Fact]
    public void ValidateUserName_ShouldReturnTrue_WhenUserNameIsValid()
    {
        // Arrange
        string validUserName = "ValidName123"; // Atende todos os critérios

        // Act
        bool result = IUser.ValidateUserName(validUserName);

        // Assert
        Assert.True(result);
    }
}