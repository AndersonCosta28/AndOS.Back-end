using AndOS.Application.Accounts.Get.GetById;
using AndOS.Infrastructure.Repositories;
using Common.Fixtures;
using Moq;

namespace Integration.Infraestructure.Repositories;
[Collection(nameof(NoParallelizationCollection))]
public class AccountRepositoryTests : IClassFixture<AccountFixture>, IAsyncLifetime
{
    private readonly AccountRepository _accountRepository;
    private readonly AccountFixture _accountFixture;
    private readonly DatabaseTest _dbTest;
    private readonly Mock<IMapperService> _mapperService;

    public AccountRepositoryTests(AccountFixture accountFixture)
    {
        _accountFixture = accountFixture;
        _dbTest = new DatabaseTest(_accountFixture.DefaultAccount.User);
        _mapperService = new Mock<IMapperService>();
        _accountRepository = new(_dbTest.DbContext, _mapperService.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAccount()
    {
        // Arrange
        var account = _accountFixture.DefaultAccount;

        // Act
        await _accountRepository.AddAsync(account);

        // Assert
        var addedAccount = await _accountRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(account.Id));
        Assert.NotNull(addedAccount);
        Assert.Equal(account.Name, addedAccount.Name);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddAccounts()
    {
        // Arrange
        var amounts = 2;
        var accounts = _accountFixture.GetAccounts(amounts);

        // Act
        await _accountRepository.AddRangeAsync(accounts);

        // Assert
        var addedAccounts = await _accountRepository.ListAsync();

        Assert.Equal(amounts, addedAccounts.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAccount()
    {
        // Arrange
        var account = _accountFixture.DefaultAccount;
        await _accountRepository.AddAsync(account);

        // Act
        account.UpdateName("Updated Account");
        await _accountRepository.UpdateAsync(account);

        // Assert
        var updatedAccount = await _accountRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(account.Id));
        Assert.Equal("Updated Account", updatedAccount.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAccount()
    {
        // Arrange
        var account = _accountFixture.DefaultAccount;
        await _accountRepository.AddAsync(account);

        // Act
        await _accountRepository.DeleteAsync(account);

        // Assert
        var deletedAccount = await _accountRepository.FirstOrDefaultAsync(new GetAccountByIdSpec(account.Id));
        Assert.Null(deletedAccount);
    }

    [Fact]
    public async Task DeleteRangeAsync_ShouldDeleteAccounts()
    {
        // Arrange
        var amount = 2;
        var accounts = _accountFixture.GetAccounts(amount);
        await _accountRepository.AddRangeAsync(accounts);

        // Act
        await _accountRepository.DeleteRangeAsync(accounts);

        // Assert
        var remainingAccounts = await _accountRepository.ListAsync();
        Assert.Empty(remainingAccounts);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var amount = 2;
        var accounts = _accountFixture.GetAccounts(amount);

        // Act
        await _accountRepository.AddRangeAsync(accounts);

        // Assert
        var result = await _accountRepository.ListAsync();
        Assert.Equal(amount, result.Count);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnAccountCount()
    {
        // Arrange
        var amount = 2;
        var accounts = _accountFixture.GetAccounts(amount);

        // Act
        await _accountRepository.AddRangeAsync(accounts);

        // Assert
        var count = await _accountRepository.CountAsync();
        Assert.Equal(amount, count);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueIfAnyAccountExists()
    {
        // Arrange
        var account = _accountFixture.DefaultAccount;

        // Act
        await _accountRepository.AddAsync(account);

        // Assert
        var exists = await _accountRepository.AnyAsync();
        Assert.True(exists);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalseIfNoAccountExists()
    {
        // Act & Assert
        var exists = await _accountRepository.AnyAsync();
        Assert.False(exists);
    }

    public async Task DisposeAsync()
    {
        _dbTest.DbContext.Accounts.RemoveRange(_dbTest.DbContext.Accounts);
        _dbTest.DbContext.Files.RemoveRange(_dbTest.DbContext.Files);
        _dbTest.DbContext.Folders.RemoveRange(_dbTest.DbContext.Folders);
        await _dbTest.DbContext.SaveChangesAsync();
    }

    public Task InitializeAsync()
    {
        _dbTest.DbContext.Accounts.RemoveRange(_dbTest.DbContext.Accounts);

        return Task.CompletedTask;
    }
}