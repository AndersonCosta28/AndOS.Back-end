using AndOS.Application.UserPreferences.Common;
using AndOS.Application.UserPreferences.Get;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;

namespace Unit.Application.UserPreferences.Get;

public class GetDefaultProgramByExtensionHandlerTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly Mock<IRepository<UserPreference>> _mockRepository;
    private readonly Mock<ICurrentUserContext> _mockCurrentUserContext;
    private readonly GetDefaultProgramByExtensionHandler _handler;

    public GetDefaultProgramByExtensionHandlerTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _mockRepository = new Mock<IRepository<UserPreference>>();
        _mockCurrentUserContext = new Mock<ICurrentUserContext>();
        _handler = new GetDefaultProgramByExtensionHandler(_mockRepository.Object, _mockCurrentUserContext.Object);
    }

    [Fact]
    public async Task Handle_PreferenceDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var request = new GetDefaultProgramByExtensionRequest("txt");

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("txt", response.DefaultProgramToExtension.Extension);
        Assert.Equal(string.Empty, response.DefaultProgramToExtension.Program);
    }

    [Fact]
    public async Task Handle_PreferenceExists_ExtensionPresent_ShouldReturnProgram()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        userPreference.UpdateDefaultProgramToExtension(new List<DefaultProgramForExtension> { new("txt", "notepad") });
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new GetDefaultProgramByExtensionRequest("txt");

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("txt", response.DefaultProgramToExtension.Extension);
        Assert.Equal("notepad", response.DefaultProgramToExtension.Program);
    }

    [Fact]
    public async Task Handle_PreferenceExists_ExtensionNotPresent_ShouldReturnDefault()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new GetDefaultProgramByExtensionRequest("txt");

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("txt", response.DefaultProgramToExtension.Extension);
        Assert.Equal(string.Empty, response.DefaultProgramToExtension.Program);
    }
}
