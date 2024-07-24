using AndOS.Domain.Exceptions.FileExceptions;
using Common.Fixtures;

namespace Unit.Domain;

public class FileTests
{
    private readonly FileFixture _fileFixture;

    public FileTests()
    {
        _fileFixture = new FileFixture();
    }

    /// <summary>
    /// Verifica se a instância da classe File é inicializada corretamente com os valores esperados.
    /// </summary>
    [Fact]
    public void File_Should_Initialize_Correctly()
    {
        // Arrange
        var folder = _fileFixture.DefaultFile.ParentFolder;
        var user = _fileFixture.DefaultFile.Owner;

        // Act
        var file = new File("TestFile", "txt", folder, user, "icon", "1KB");

        // Assert
        Assert.Equal("TestFile", file.Name);
        Assert.Equal("txt", file.Extension);
        Assert.Equal(folder, file.ParentFolder);
        Assert.Equal(user, file.Owner);
        Assert.Equal("icon", file.Icon);
        Assert.Equal("1KB", file.Size);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFileNameLengthException é lançada ao tentar atualizar o nome do arquivo com um nome de comprimento inválido.
    /// </summary>
    [Fact]
    public void UpdateName_Should_Throw_Exception_For_Invalid_Name_Length()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;

        // Act & Assert
        Assert.Throws<InvalidFileNameLengthException>(() => file.UpdateName(""));
    }

    /// <summary>
    /// Verifica se o nome do arquivo é atualizado corretamente.
    /// </summary>
    [Fact]
    public void UpdateName_Should_Update_Name_Correctly()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        var newName = "NewFileName";

        // Act
        file.UpdateName(newName);

        // Assert
        Assert.Equal(newName, file.Name);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFileNameCharacters é lançada ao tentar validar um nome de arquivo com caracteres inválidos.
    /// </summary>
    [Fact]
    public void ValidateName_Should_Throw_Exception_For_Invalid_Name_Characters()
    {
        // Act & Assert
        Assert.Throws<InvalidFileNameCharacters>(() => File.ValidateName("Invalid/Name"));
    }

    /// <summary>
    /// Verifica se a validação de um nome de arquivo válido retorna true.
    /// </summary>
    [Fact]
    public void ValidateName_Should_Return_True_For_Valid_Name()
    {
        // Act
        var result = File.ValidateName("ValidName");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFileExtensionLengthException é lançada ao tentar atualizar a extensão do arquivo com uma extensão de comprimento inválido.
    /// </summary>
    [Fact]
    public void UpdateExtension_Should_Throw_Exception_For_Invalid_Extension_Length()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;

        // Act & Assert
        Assert.Throws<InvalidFileExtensionLengthException>(() => file.UpdateExtension(""));
    }

    /// <summary>
    /// Verifica se a extensão do arquivo é atualizada corretamente.
    /// </summary>
    [Fact]
    public void UpdateExtension_Should_Update_Extension_Correctly()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        var newExtension = "pdf";

        // Act
        file.UpdateExtension(newExtension);

        // Assert
        Assert.Equal(newExtension, file.Extension);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFileExtensionCharacters é lançada ao tentar validar uma extensão de arquivo com caracteres inválidos.
    /// </summary>
    [Fact]
    public void ValidateExtension_Should_Throw_Exception_For_Invalid_Extension_Characters()
    {
        // Act & Assert
        Assert.Throws<InvalidFileExtensionCharacters>(() => File.ValidateExtension("/xt"));
    }

    /// <summary>
    /// Verifica se a validação de uma extensão de arquivo válida retorna true.
    /// </summary>
    [Fact]
    public void ValidateExtension_Should_Return_True_For_Valid_Extension()
    {
        // Act
        var result = File.ValidateExtension("txt");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifica se o tamanho do arquivo é atualizado corretamente.
    /// </summary>
    [Fact]
    public void UpdateSize_Should_Update_Size_Correctly()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        var newSize = "2MB";

        // Act
        file.UpdateSize(newSize);

        // Assert
        Assert.Equal(newSize, file.Size);
    }

    /// <summary>
    /// Verifica se a pasta pai do arquivo é atualizada corretamente.
    /// </summary>
    [Fact]
    public void UpdateParentFolder_Should_Update_ParentFolder_Correctly()
    {
        // Arrange
        var file = _fileFixture.DefaultFile;
        var newParentFolder = _fileFixture.DefaultFile.ParentFolder;

        // Act
        file.UpdateParentFolder(newParentFolder);

        // Assert
        Assert.Equal(newParentFolder, file.ParentFolder);
    }
}
