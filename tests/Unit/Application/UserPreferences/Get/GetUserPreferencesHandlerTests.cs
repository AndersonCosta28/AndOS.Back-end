using AndOS.Application.UserPreferences.Common;
using AndOS.Application.UserPreferences.Get;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.UserPreferences.Get;

namespace Unit.Application.UserPreferences.Get;

public class GetUserPreferencesHandlerTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>
{
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly Mock<IRepository<UserPreference>> _mockRepository;
    private readonly Mock<ICurrentUserContext> _mockCurrentUserContext;
    private readonly Mock<IMapperService> _mockMapperService;
    private readonly GetUserPreferencesHandler _handler;

    public GetUserPreferencesHandlerTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _mockRepository = new Mock<IRepository<UserPreference>>();
        _mockCurrentUserContext = new Mock<ICurrentUserContext>();
        _mockMapperService = new Mock<IMapperService>();
        _handler = new GetUserPreferencesHandler(_mockRepository.Object, _mockCurrentUserContext.Object, _mockMapperService.Object);
    }

    [Fact]
    public async Task Handle_PreferenceDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var request = new GetUserPreferencesRequest();

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal("en-US", response.Language);
        Assert.Empty(response.DefaultProgramsToExtensions);
    }

    [Fact]
    public async Task Handle_PreferenceExists_ShouldReturnMappedPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        userPreference.UpdateLanguage("fr-FR");
        userPreference.UpdateDefaultProgramToExtension(new List<DefaultProgramForExtension> { new("txt", "notepad") });
        _mockRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetUserPreferenceByUserIdSpec>(), CancellationToken.None))
            .ReturnsAsync(userPreference);
        var request = new GetUserPreferencesRequest();
        var mappedResponse = new UserPreferenceDTO("fr-FR", new List<DefaultProgramToExtensionDTO> { new("txt", "notepad") });
        _mockMapperService.Setup(service => service.MapAsync<UserPreferenceDTO>(userPreference))
            .ReturnsAsync(mappedResponse);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(mappedResponse, response);
    }
}
