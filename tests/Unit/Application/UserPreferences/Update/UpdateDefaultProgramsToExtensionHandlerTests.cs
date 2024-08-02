using AndOS.Application.UserPreferences.Common;
using AndOS.Application.UserPreferences.Update;
using AndOS.Shared.Requests.UserPreferences.Update;

namespace Unit.Application.UserPreferences.Update;

public class UpdateDefaultProgramsToExtensionHandlerTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly Mock<IRepository<UserPreference>> _mockRepository;
    private readonly Mock<ICurrentUserContext> _mockCurrentUserContext;
    private readonly Mock<IMapperService> _mockMapperService;
    private readonly UpdateDefaultProgramsToExtensionHandler _handler;

    public UpdateDefaultProgramsToExtensionHandlerTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _mockRepository = new Mock<IRepository<UserPreference>>();
        _mockCurrentUserContext = new Mock<ICurrentUserContext>();
        _mockMapperService = new Mock<IMapperService>();
        _handler = new UpdateDefaultProgramsToExtensionHandler(_mockRepository.Object, _mockCurrentUserContext.Object, _mockMapperService.Object);
    }

    [Fact]
    public async Task Handle_PreferenceDoesNotExist_ShouldAddNewPreference()
    {
        // Arrange
        var request = new UpdateDefaultProgramsToExtensionRequest([new("txt", "notepad")]);
        var entity = new List<DefaultProgramForExtension>() { new("txt", "notepad") };
        _mockMapperService.Setup(x => x.MapAsync<List<DefaultProgramForExtension>>(request.DefaultProgramsToExtensions)).ReturnsAsync(entity);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<UserPreference>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_PreferenceExists_ShouldUpdateExistingPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new UpdateDefaultProgramsToExtensionRequest([new("txt", "notepad")]);
        var entity = new List<DefaultProgramForExtension>() { new("txt", "notepad") };
        _mockMapperService.Setup(x => x.MapAsync<List<DefaultProgramForExtension>>(request.DefaultProgramsToExtensions)).ReturnsAsync(entity);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UserPreference>(), CancellationToken.None), Times.Once);
    }
}
