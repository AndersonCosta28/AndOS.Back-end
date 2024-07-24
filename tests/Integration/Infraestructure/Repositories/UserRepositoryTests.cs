using AndOS.Application.Users.Get.GetById;
using AndOS.Infrastructure.Repositories;
using Ardalis.Specification;
using Common.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Integration.Infraestructure.Repositories;
[Collection(nameof(NoParallelizationCollection))]
public class UserRepositoryTests : IClassFixture<UserFixture>, IAsyncLifetime
{
    private readonly UserRepository _userRepository;
    private readonly DatabaseTest _dbTest;
    private readonly UserFixture _userFixture;
    private readonly Mock<IMapperService> _mapperService;

    public UserRepositoryTests(UserFixture userFixture)
    {
        _dbTest = new DatabaseTest();
        _userFixture = userFixture;
        _mapperService = new Mock<IMapperService>();
        _userRepository = new(_dbTest.DbContext, _mapperService.Object);
    }

    [Fact]
    public async Task AddUser_ShouldAddUserToDatabase()
    {
        var user = _userFixture.CreateNewUser();
        await _userRepository.AddAsync(user);

        var spec = new GetUserByIdSpec(user.Id);
        var retrievedUser = await _userRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedUser);
        Assert.Equal(user.UserName, retrievedUser.UserName);

    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserInDatabase()
    {
        var user = _userFixture.CreateNewUser();
        await _userRepository.AddAsync(user);

        user.UserName = "Updated User";
        await _userRepository.UpdateAsync(user);

        var spec = new GetUserByIdSpec(user.Id);
        var retrievedUser = await _userRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedUser);
        Assert.Equal("Updated User", retrievedUser.UserName);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUserFromDatabase()
    {
        var user = _userFixture.CreateNewUser();
        await _userRepository.AddAsync(user);
        await _userRepository.DeleteAsync(user);

        var spec = new GetUserByIdSpec(user.Id);
        var retrievedUser = await _userRepository.FirstOrDefaultAsync(spec);

        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleUsersToDatabase()
    {
        var users = _userFixture.GetUsers(2);
        await _userRepository.AddRangeAsync(users);

        foreach (var user in users)
        {
            var spec = new GetUserByIdSpec(user.Id);
            var retrievedUser = await _userRepository.FirstOrDefaultAsync(spec);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.UserName, retrievedUser.UserName);
        }
    }

    [Fact]
    public async Task DeleteRangeAsync_ShouldRemoveMultipleUsersFromDatabase()
    {
        var users = _userFixture.GetUsers(2);
        await _userRepository.AddRangeAsync(users);

        var spec = new InMemorySpecification(users.Select(u => u.Id));
        await _userRepository.DeleteRangeAsync(specification: spec);

        foreach (var user in users)
        {
            var retrievalSpec = new GetUserByIdSpec(user.Id);
            var retrievedUser = await _userRepository.FirstOrDefaultAsync(retrievalSpec);
            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAllUsers()
    {
        var users = _userFixture.GetUsers(2);
        await _userRepository.AddRangeAsync(users);

        var allUsers = await _userRepository.ListAsync();
        Assert.Equal(users.Count, allUsers.Count());
        Assert.True(users.All(u => allUsers.Any(au => au.UserName == u.UserName && au.UserName == u.UserName)));
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectNumberOfUsers()
    {
        var users = _userFixture.GetUsers(2);
        await _userRepository.AddRangeAsync(users);

        var count = await _userRepository.CountAsync();
        Assert.Equal(users.Count, count);
    }

    [Fact]
    public async Task AnyAsync_ShouldIndicateIfThereAreUsers()
    {
        var users = _userFixture.GetUsers(2);
        await _userRepository.AddRangeAsync(users);

        var anyUsers = await _userRepository.AnyAsync();
        Assert.True(anyUsers);
    }


    public async Task DisposeAsync()
    {
        _dbTest.DbContext.Users.RemoveRange(_dbTest.DbContext.Users);
        await _dbTest.DbContext.SaveChangesAsync();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}

// Implementação de uma especificação genérica para filtragem por IDs, assumindo que você precisa de algo assim
public class InMemorySpecification : Specification<IUser>
{
    private readonly IEnumerable<Guid> _ids;

    public InMemorySpecification(IEnumerable<Guid> ids)
    {
        _ids = ids;
        Query.Where(x => _ids.Contains(x.Id));
    }
}