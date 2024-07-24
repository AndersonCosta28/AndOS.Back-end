using AndOS.Application.Files.Get.GetById;
using AndOS.Infrastructure.Repositories;
using Common.Fixtures;
using Moq;

namespace Integration.Infraestructure.Repositories;
[Collection(nameof(NoParallelizationCollection))]

public class FileRepositoryTests : IClassFixture<FileFixture>, IAsyncLifetime
{
    private readonly DatabaseTest _dbTest;
    private readonly Mock<IMapperService> _mapperService;
    private readonly FileRepository _fileRepository;
    private readonly FileFixture _fileFixture;

    public FileRepositoryTests(FileFixture fileFixture)
    {
        _dbTest = new DatabaseTest(fileFixture.DefaultFile.GetUser());
        _mapperService = new Mock<IMapperService>();

        _fileRepository = new(_dbTest.DbContext, _mapperService.Object);

        _fileFixture = fileFixture;
    }

    /// <summary>
    /// Testa a adição de um arquivo ao banco de dados e verifica se ele pode ser recuperado corretamente.
    /// </summary>
    [Fact]
    public async Task AddFile_ShouldAddFileToDatabase()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;

        // Act
        await _fileRepository.AddAsync(file);

        // Assert
        var spec = new GetFileByIdSpec(file.Id);
        var retrievedFile = await _fileRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedFile);
        Assert.Equal(file.Name, retrievedFile.Name);
    }

    /// <summary>
    /// Testa a atualização de um arquivo no banco de dados e verifica se a atualização foi aplicada corretamente.
    /// </summary>
    [Fact]
    public async Task UpdateFile_ShouldUpdateFileInDatabase()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        await _fileRepository.AddAsync(file);

        // Act
        file.UpdateName("updatedfile.txt");
        file.UpdateExtension("doc");
        file.UpdateSize("2KB");
        await _fileRepository.UpdateAsync(file);

        // Assert
        var spec = new GetFileByIdSpec(file.Id);
        var retrievedFile = await _fileRepository.FirstOrDefaultAsync(spec);

        Assert.NotNull(retrievedFile);
        Assert.Equal("updatedfile.txt", retrievedFile.Name);
        Assert.Equal("doc", retrievedFile.Extension);
        Assert.Equal("2KB", retrievedFile.Size);
    }

    /// <summary>
    /// Testa a remoção de um arquivo do banco de dados e verifica se ele foi removido.
    /// </summary>
    [Fact]
    public async Task DeleteFile_ShouldRemoveFileFromDatabase()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        await _fileRepository.AddAsync(file);

        // Act
        await _fileRepository.DeleteAsync(file);

        // Assert
        var spec = new GetFileByIdSpec(file.Id);
        var retrievedFile = await _fileRepository.FirstOrDefaultAsync(spec);

        Assert.Null(retrievedFile);
    }

    /// <summary>
    /// Testa a adição de múltiplos arquivos ao banco de dados e verifica se todos podem ser recuperados.
    /// </summary>
    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleFilesToDatabase()
    {
        // Arrange
        var files = _fileFixture.GetFiles(2);

        // Act
        await _fileRepository.AddRangeAsync(files);

        // Assert
        foreach (var file in files)
        {
            var spec = new GetFileByIdSpec(file.Id);
            var retrievedFile = await _fileRepository.FirstOrDefaultAsync(spec);
            Assert.NotNull(retrievedFile);
            Assert.Equal(file.Name, retrievedFile.Name);
        }
    }

    /// <summary>
    /// Testa a listagem de todos os arquivos e verifica se todos os arquivos adicionados estão presentes.
    /// </summary>
    [Fact]
    public async Task ListAsync_ShouldReturnAllFiles()
    {
        // Arrange
        var files = _fileFixture.GetFiles(2);
        await _fileRepository.AddRangeAsync(files);

        // Act
        var allFiles = await _fileRepository.ListAsync();

        // Assert
        Assert.Equal(files.Count, allFiles.Count());
        Assert.True(files.All(f => allFiles.Any(af => af.Name == f.Name && af.Extension == f.Extension)));
    }

    /// <summary>
    /// Testa a contagem de arquivos e verifica se corresponde ao número esperado.
    /// </summary>
    [Fact]
    public async Task CountAsync_ShouldReturnCorrectNumberOfFiles()
    {
        // Arrange
        var files = _fileFixture.GetFiles(2);
        await _fileRepository.AddRangeAsync(files);

        // Act
        var count = await _fileRepository.CountAsync();

        // Assert
        Assert.Equal(files.Count, count);
    }

    /// <summary>
    /// Testa se há arquivos no banco de dados após adicionar alguns.
    /// </summary>
    [Fact]
    public async Task AnyAsync_ShouldIndicateIfThereAreFiles()
    {
        // Arrange
        var files = _fileFixture.GetFiles(2);
        await _fileRepository.AddRangeAsync(files);

        // Act
        var anyFiles = await _fileRepository.AnyAsync();

        // Assert
        Assert.True(anyFiles);
    }

    public async Task DisposeAsync()
    {
        _dbTest.DbContext.Files.RemoveRange(_dbTest.DbContext.Files);
        await _dbTest.DbContext.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        _dbTest.DbContext.Files.RemoveRange(_dbTest.DbContext.Files);
        await _dbTest.DbContext.SaveChangesAsync();
    }
}