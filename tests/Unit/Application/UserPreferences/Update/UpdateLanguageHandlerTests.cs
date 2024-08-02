using AndOS.Application.UserPreferences.Common;
using AndOS.Application.UserPreferences.Update;
using AndOS.Shared.Requests.UserPreferences.Update;

namespace Unit.Application.UserPreferences.Update;

public class UpdateLanguageHandlerTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly Mock<IRepository<UserPreference>> _mockRepository;
    private readonly Mock<ICurrentUserContext> _mockCurrentUserContext;
    private readonly UpdateLanguageHandler _handler;

    public UpdateLanguageHandlerTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _mockRepository = new Mock<IRepository<UserPreference>>();
        _mockCurrentUserContext = new Mock<ICurrentUserContext>();
        _handler = new UpdateLanguageHandler(_mockRepository.Object, _mockCurrentUserContext.Object);
    }

    [Fact]
    public async Task Handle_PreferenceDoesNotExist_ShouldAddNewPreferenceWithLanguage()
    {
        // Arrange
        var request = new UpdateLanguageRequest("fr-FR");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<UserPreference>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PreferenceExists_ShouldUpdateExistingPreferenceWithLanguage()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new UpdateLanguageRequest("es-ES");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UserPreference>(), CancellationToken.None), Times.Once);
    }
}
