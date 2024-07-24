using AndOS.Application.Folders.Get.GetById;
using AndOS.Infrastructure.Repositories;
using Common.Fixtures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Integration.Infraestructure.Repositories;
[Collection(nameof(NoParallelizationCollection))]

public class FolderRepositoryTests : IClassFixture<FolderFixture>, IAsyncLifetime
{
    private readonly DatabaseTest _dbTest;
    private readonly AccountRepository _accountRepository;
    private readonly FolderRepository _folderRepository;
    private readonly FolderFixture _folderFixture;
    private readonly Mock<IMapperService> _mapperService;

    List<Folder> _foldersPreSavings;
    public FolderRepositoryTests(FolderFixture folderFixture)
    {
        _folderFixture = folderFixture;
        _foldersPreSavings = [_folderFixture.CommonFolder, _folderFixture.UserFolder, _folderFixture.StorageFolder];
        _dbTest = new DatabaseTest(_folderFixture.User);
        _mapperService = new Mock<IMapperService>();
        _accountRepository = new(_dbTest.DbContext, _mapperService.Object);
        _folderRepository = new(_dbTest.DbContext, _mapperService.Object);
    }

    /// <summary>
    /// Testa a adição de uma pasta ao banco de dados e verifica se ela pode ser recuperada corretamente.
    /// </summary>
    [Fact]
    public async Task AddFolder_ShouldAddFolderToDatabase()
    {
        // Arrange
        var folder = _folderFixture.CreateNewCommonFolder();

        // Act
        await _folderRepository.AddAsync(folder);

        // Assert
        var spec = new GetFolderByIdSpec(folder.Id);
        var retrievedFolder = await _folderRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedFolder);
        Assert.Equal(folder.Name, retrievedFolder.Name);
    }

    /// <summary>
    /// Testa a adição de múltiplas pastas ao banco de dados e verifica se todas podem ser recuperadas.
    /// </summary>
    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleFoldersToDatabase()
    {
        // Arrange
        var folders = _folderFixture.GetFolders(3);

        // Act
        await _folderRepository.AddRangeAsync(folders);

        // Assert
        foreach (var folder in folders)
        {
            var spec = new GetFolderByIdSpec(folder.Id);
            var retrievedFolder = await _folderRepository.FirstOrDefaultAsync(spec);
            Assert.NotNull(retrievedFolder);
            Assert.Equal(folder.Name, retrievedFolder.Name);
        }
    }

    /// <summary>
    /// Testa se há pastas no banco de dados após adicionar algumas.
    /// </summary>
    [Fact]
    public async Task AnyAsync_ShouldIndicateIfThereAreFolders()
    {
        // Arrange
        var folders = _folderFixture.GetFolders(3);
        await _folderRepository.AddRangeAsync(folders);

        // Act
        var anyFolders = await _folderRepository.AnyAsync();

        // Assert
        Assert.True(anyFolders);
    }

    /// <summary>
    /// Testa a contagem de pastas e verifica se corresponde ao número esperado.
    /// </summary>
    [Fact]
    public async Task CountAsync_ShouldReturnCorrectNumberOfFolders()
    {
        // Arrange
        var folders = _folderFixture.GetFolders(3);

        await _folderRepository.AddRangeAsync(folders);

        // Act
        var count = await _folderRepository.CountAsync();

        // Assert
        Assert.Equal(folders.Count + _foldersPreSavings.Count, count);
    }

    /// <summary>
    /// Testa a remoção de uma pasta do banco de dados e verifica se ela foi removida.
    /// </summary>
    [Fact]
    public async Task DeleteFolder_ShouldRemoveFolderFromDatabase()
    {
        // Arrange
        var folder = _folderFixture.CreateNewCommonFolder();
        await _folderRepository.AddAsync(folder);

        // Act
        await _folderRepository.DeleteAsync(folder);

        // Assert
        var spec = new GetFolderByIdSpec(folder.Id);
        var retrievedFolder = await _folderRepository.FirstOrDefaultAsync(spec);

        Assert.Null(retrievedFolder);
    }

    /// <summary>
    /// Testa a listagem de todas as pastas e verifica se todas as pastas adicionadas estão presentes.
    /// </summary>
    [Fact]
    public async Task ListAsync_ShouldReturnAllFolders()
    {
        // Arrange
        var folders = _folderFixture.GetFolders(3);
        await _folderRepository.AddRangeAsync(folders);

        // Act
        var allFolders = await _folderRepository.ListAsync();

        // Assert
        Assert.Equal(folders.Count + _foldersPreSavings.Count, allFolders.Count);
        Assert.True(folders.All(f => allFolders.Any(af => af.Name == f.Name)));
    }

    /// <summary>
    /// Testa a atualização de uma pasta no banco de dados e verifica se a atualização foi aplicada corretamente.
    /// </summary>
    [Fact]
    public async Task UpdateFolder_ShouldUpdateFolderInDatabase()
    {
        // Arrange
        var folder = _folderFixture.CreateNewCommonFolder();
        await _folderRepository.AddAsync(folder);

        // Act
        folder.UpdateName("Updated Folder");
        await _folderRepository.UpdateAsync(folder);

        // Assert
        var spec = new GetFolderByIdSpec(folder.Id);
        var retrievedFolder = await _folderRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedFolder);
        Assert.Equal("Updated Folder", retrievedFolder.Name);
    }

    public Task DisposeAsync() => DeleteFolders();

    public Task InitializeAsync() => DeleteFolders();

    async Task DeleteFolders()
    {
        var folders = await _dbTest.DbContext.Folders.ToListAsync();
        var foldersToRemove = folders.Where(folder => !_foldersPreSavings.Contains(folder)).ToList();
        _dbTest.DbContext.Folders.RemoveRange(foldersToRemove);
        await _dbTest.DbContext.SaveChangesAsync();
    }
}