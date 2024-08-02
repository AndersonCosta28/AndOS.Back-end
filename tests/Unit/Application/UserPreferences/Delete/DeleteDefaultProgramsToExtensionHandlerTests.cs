using AndOS.Application.UserPreferences.Common;
using AndOS.Application.UserPreferences.Delete;
using AndOS.Shared.Requests.UserPreferences.Delete;

namespace Unit.Application.UserPreferences.Delete;

public class DeleteDefaultProgramsToExtensionHandlerTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly Mock<IRepository<UserPreference>> _mockRepository;
    private readonly Mock<ICurrentUserContext> _mockCurrentUserContext;
    private readonly DeleteDefaultProgramsToExtensionHandler _handler;

    public DeleteDefaultProgramsToExtensionHandlerTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _mockRepository = new Mock<IRepository<UserPreference>>();
        _mockCurrentUserContext = new Mock<ICurrentUserContext>();
        _handler = new DeleteDefaultProgramsToExtensionHandler(_mockRepository.Object, _mockCurrentUserContext.Object);
    }

    [Fact]
    public async Task Handle_PreferenceDoesNotExist_ShouldNotThrowException()
    {
        // Arrange
        var request = new DeleteDefaultProgramToExtensionRequest("txt");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        // No exceptions should be thrown
    }

    [Fact]
    public async Task Handle_PreferenceExists_ExtensionPresent_ShouldRemoveExtension()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        userPreference.UpdateDefaultProgramToExtension([new("txt", "notepad")]);
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new DeleteDefaultProgramToExtensionRequest("txt");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Null(userPreference.DefaultProgramsToExtensions.Find(x => x.Extension == "txt"));
    }

    [Fact]
    public async Task Handle_PreferenceExists_ExtensionNotPresent_ShouldNotModifyPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new DeleteDefaultProgramToExtensionRequest("txt");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Empty(userPreference.DefaultProgramsToExtensions);
    }
}
