using AndOS.Application.UserPreferences.Common;
using AndOS.Infrastructure.Repositories;
using Common.Fixtures;
using Moq;

namespace Integration.Infraestructure.Repositories;
[Collection(nameof(NoParallelizationCollection))]
public class UserPreferenceRepositoryTests : IClassFixture<UserPreferenceFixture>, IClassFixture<UserFixture>, IAsyncLifetime
{
    private readonly UserPreferenceRepository _userPreferenceRepository;
    private readonly UserPreferenceFixture _userPreferenceFixture;
    private readonly UserFixture _userFixture;
    private readonly DatabaseTest _dbTest;
    private readonly Mock<IMapperService> _mapperService;

    public UserPreferenceRepositoryTests(UserPreferenceFixture userPreferenceFixture, UserFixture userFixture)
    {
        _userPreferenceFixture = userPreferenceFixture;
        _userFixture = userFixture;
        _dbTest = new DatabaseTest(_userFixture.DefaultUser);
        _mapperService = new Mock<IMapperService>();
        _userPreferenceRepository = new(_dbTest.DbContext, _mapperService.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);

        // Act
        await _userPreferenceRepository.AddAsync(userPreference);

        // Assert
        var addedUserPreference = await _userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(userPreference.UserId));
        Assert.NotNull(addedUserPreference);
        Assert.Equal(userPreference.Language, addedUserPreference.Language);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddUserPreferences()
    {
        // Arrange
        var amounts = 2;
        var userPreferences = _userPreferenceFixture.GetUserPreferences(amounts);

        // Act
        await _userPreferenceRepository.AddRangeAsync(userPreferences);

        // Assert
        var addedUserPreferences = await _userPreferenceRepository.ListAsync();

        Assert.Equal(amounts, addedUserPreferences.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        await _userPreferenceRepository.AddAsync(userPreference);

        // Act
        userPreference.UpdateLanguage("pt-BR");
        await _userPreferenceRepository.UpdateAsync(userPreference);

        // Assert
        var updatedUserPreference = await _userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(userPreference.UserId));
        Assert.Equal("pt-BR", updatedUserPreference.Language);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUserPreference()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);
        await _userPreferenceRepository.AddAsync(userPreference);

        // Act
        await _userPreferenceRepository.DeleteAsync(userPreference);

        // Assert
        var deletedUserPreference = await _userPreferenceRepository.FirstOrDefaultAsync(new GetUserPreferenceByUserIdSpec(userPreference.UserId));
        Assert.Null(deletedUserPreference);
    }

    [Fact]
    public async Task DeleteRangeAsync_ShouldDeleteUserPreferences()
    {
        // Arrange
        var amount = 2;
        var userPreferences = _userPreferenceFixture.GetUserPreferences(amount);
        await _userPreferenceRepository.AddRangeAsync(userPreferences);

        // Act
        await _userPreferenceRepository.DeleteRangeAsync(userPreferences);

        // Assert
        var remainingUserPreferences = await _userPreferenceRepository.ListAsync();
        Assert.Empty(remainingUserPreferences);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAllUserPreferences()
    {
        // Arrange
        var amount = 2;
        var userPreferences = _userPreferenceFixture.GetUserPreferences(amount);

        // Act
        await _userPreferenceRepository.AddRangeAsync(userPreferences);

        // Assert
        var result = await _userPreferenceRepository.ListAsync();
        Assert.Equal(amount, result.Count);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnUserPreferenceCount()
    {
        // Arrange
        var amount = 2;
        var userPreferences = _userPreferenceFixture.GetUserPreferences(amount);

        // Act
        await _userPreferenceRepository.AddRangeAsync(userPreferences);

        // Assert
        var count = await _userPreferenceRepository.CountAsync();
        Assert.Equal(amount, count);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueIfAnyUserPreferenceExists()
    {
        // Arrange
        var userPreference = _userPreferenceFixture.CreateUserPreference(_userFixture.DefaultUser);

        // Act
        await _userPreferenceRepository.AddAsync(userPreference);

        // Assert
        var exists = await _userPreferenceRepository.AnyAsync();
        Assert.True(exists);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalseIfNoUserPreferenceExists()
    {
        // Act & Assert
        var exists = await _userPreferenceRepository.AnyAsync();
        Assert.False(exists);
    }

    public async Task DisposeAsync()
    {
        _dbTest.DbContext.UserPreferences.RemoveRange(_dbTest.DbContext.UserPreferences);
        await _dbTest.DbContext.SaveChangesAsync();
    }

    public Task InitializeAsync()
    {
        _dbTest.DbContext.UserPreferences.RemoveRange(_dbTest.DbContext.UserPreferences);
        return Task.CompletedTask;
    }
}