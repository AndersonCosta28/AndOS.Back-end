using AndOS.Application.Files.Common.Specs;
using AndOS.Application.Files.Create;
using AndOS.Shared.Requests.Files.Create;

namespace Unit.Application.Files.Create;

public class CreateFileValidatorTests
{
    private readonly CreateFileValidator _validator;
    private readonly Mock<IReadRepository<File>> _fileRepositoryMock;
    private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;

    public CreateFileValidatorTests()
    {
        _fileRepositoryMock = new Mock<IReadRepository<File>>();
        _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

        // Setup localization messages
        _localizerMock.Setup(l => l["RequiredFileName"]).Returns(new LocalizedString("RequiredFileName", "The file name is required"));
        _localizerMock.Setup(l => l["FileNameLength"]).Returns(new LocalizedString("FileNameLength", "The file name must be between 1 and 100 characters long"));
        _localizerMock.Setup(l => l["InvalidFileNameCharacters"]).Returns(new LocalizedString("InvalidFileNameCharacters", "The file name cannot contain invalid characters"));
        _localizerMock.Setup(l => l["FileNameAlreadyExistsInDirectory"]).Returns(new LocalizedString("FileNameAlreadyExistsInDirectory", "A file with this name already exists in the specified directory"));

        _validator = new CreateFileValidator(_fileRepositoryMock.Object, _localizerMock.Object);
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o nome do arquivo é nulo.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Is_Null()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = null, ParentFolderId = Guid.NewGuid() };

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The file name is required");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o nome do arquivo é uma string vazia.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "", ParentFolderId = Guid.NewGuid() };

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The file name is required");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o nome do arquivo é uma string contendo apenas espaços em branco.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Is_Whitespace()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = " ", ParentFolderId = Guid.NewGuid() };

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The file name is required");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o comprimento do nome do arquivo é menor que 1 caractere.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Is_Too_Short()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "", ParentFolderId = Guid.NewGuid() }; // Less than 1 character

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("The file name must be between 1 and 100 characters long");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o comprimento do nome do arquivo é maior que 100 caracteres.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Is_Too_Long()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = new string('a', 101), ParentFolderId = Guid.NewGuid() }; // More than 100 characters

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("The file name must be between 1 and 100 characters long");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o nome do arquivo contém uma barra (/).
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Has_Invalid_Characters_Slash()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "invalid/name", ParentFolderId = Guid.NewGuid() };

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The file name cannot contain invalid characters");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o nome do arquivo contém uma barra invertida (\).
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Name_Has_Invalid_Characters_Backslash()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "invalid\\name", ParentFolderId = Guid.NewGuid() };

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The file name cannot contain invalid characters");
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando um arquivo com o mesmo nome já existe no diretório especificado.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_FileName_Already_Exists_In_Directory()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "ExistingFile", ParentFolderId = Guid.NewGuid() };
        _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByNameAndParentFolderIdSpec>(), default)).ReturnsAsync(true);

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor("Name/Extension").WithErrorMessage("A file with this name already exists in the specified directory");
    }

    /// <summary>
    /// Verifica se o validador não retorna nenhum erro quando a solicitação é válida.
    /// </summary>
    [Fact]
    public async Task Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        CreateFileRequest request = new CreateFileRequest { Name = "ValidFileName", ParentFolderId = Guid.NewGuid() };
        _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByNameAndParentFolderIdSpec>(), default)).ReturnsAsync(false);

        // Act
        TestValidationResult<CreateFileRequest> result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
