using AndOS.Domain.Exceptions.FolderExceptions;

namespace Unit.Domain;

public class FolderTests
{
    private readonly FolderFixture _folderFixture;

    public FolderTests()
    {
        _folderFixture = new FolderFixture();
    }

    /// <summary>
    /// Verifica se a instância da classe Folder é inicializada corretamente com os valores esperados.
    /// </summary>
    [Fact]
    public void Folder_Should_Initialize_Correctly()
    {
        // Arrange
        var user = _folderFixture.UserFolder.Owner;
        var parentFolder = _folderFixture.StorageFolder;

        // Act
        var folder = new Folder("Test Folder", user, parentFolder);

        // Assert
        Assert.Equal("Test Folder", folder.Name);
        Assert.Equal(user, folder.Owner);
        Assert.Equal(parentFolder, folder.ParentFolder);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFolderNameLengthException é lançada ao tentar atualizar o nome da pasta com um nome de comprimento inválido.
    /// </summary>
    [Fact]
    public void UpdateName_Should_Throw_Exception_For_Invalid_Name_Length()
    {
        // Arrange
        var folder = _folderFixture.UserFolder;

        // Act & Assert
        Assert.Throws<InvalidFolderNameLengthException>(() => folder.UpdateName(string.Empty));
    }

    /// <summary>
    /// Verifica se o nome da pasta é atualizado corretamente.
    /// </summary>
    [Fact]
    public void UpdateName_Should_Update_Name_Correctly()
    {
        // Arrange
        var folder = _folderFixture.UserFolder;
        var newName = "New Folder Name";

        // Act
        folder.UpdateName(newName);

        // Assert
        Assert.Equal(newName, folder.Name);
    }

    /// <summary>
    /// Verifica se a exceção InvalidFolderNameCharacters é lançada ao tentar validar um nome de pasta com caracteres inválidos.
    /// </summary>
    [Fact]
    public void ValidateName_Should_Throw_Exception_For_Invalid_Name_Characters()
    {
        // Act & Assert
        Assert.Throws<InvalidFolderNameCharacters>(() => Folder.ValidateName("Invalid/Name"));
    }

    /// <summary>
    /// Verifica se a validação de um nome de pasta válido retorna true.
    /// </summary>
    [Fact]
    public void ValidateName_Should_Return_True_For_Valid_Name()
    {
        // Act
        var result = Folder.ValidateName("ValidName");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifica se a exceção DomainLayerException é lançada ao tentar atualizar o tipo da pasta para Storage sem um ParentFolder válido.
    /// </summary>
    [Fact]
    public void UpdateType_Should_Throw_Exception_For_Invalid_ParentFolder()
    {
        // Arrange
        var folder = new Folder("Test Folder", _folderFixture.UserFolder.Owner);

        // Act & Assert
        Assert.Throws<DomainLayerException>(() => folder.UpdateType(FolderType.Storage));
    }

    /// <summary>
    /// Verifica se o tipo da pasta é atualizado corretamente.
    /// </summary>
    [Fact]
    public void UpdateType_Should_Update_Type_Correctly()
    {
        // Arrange
        var folder = _folderFixture.StorageFolder;

        // Act
        folder.UpdateType(FolderType.Common);

        // Assert
        Assert.Equal(FolderType.Common, folder.Type);
    }

    /// <summary>
    /// Verifica se a exceção DomainLayerException é lançada ao tentar atualizar a conta de uma pasta que não é do tipo Storage.
    /// </summary>
    [Fact]
    public void UpdateAccount_Should_Throw_Exception_For_Invalid_Folder_Type()
    {
        // Arrange
        var folder = _folderFixture.UserFolder;
        var account = new AccountFixture().DefaultAccount;

        // Act & Assert
        Assert.Throws<DomainLayerException>(() => folder.UpdateAccount(account));
    }

    /// <summary>
    /// Verifica se a conta da pasta é atualizada corretamente.
    /// </summary>
    [Fact]
    public void UpdateAccount_Should_Update_Account_Correctly()
    {
        // Arrange
        var folder = _folderFixture.StorageFolder;
        var account = new AccountFixture().DefaultAccount;

        // Act
        folder.UpdateAccount(account);

        // Assert
        Assert.Equal(account, folder.Account);
    }

    /// <summary>
    /// Verifica se o usuário da pasta é atualizado corretamente.
    /// </summary>
    [Fact]
    public void UpdateUser_Should_Update_User_Correctly()
    {
        // Arrange
        var folder = _folderFixture.StorageFolder;
        var user = new UserFixture().DefaultUser;

        // Act
        folder.UpdateUser(user);

        // Assert
        Assert.Equal(user, folder.User);
    }

    /// <summary>
    /// Verifica se a pasta pai é atualizada corretamente.
    /// </summary>
    [Fact]
    public void UpdateParentFolder_Should_Update_ParentFolder_Correctly()
    {
        // Arrange
        var folder = _folderFixture.CommonFolder;
        var newParentFolder = _folderFixture.CreateNewStorageFolder();

        // Act
        folder.UpdateParentFolder(newParentFolder);

        // Assert
        Assert.Equal(newParentFolder, folder.ParentFolder);
    }

    /// <summary>
    /// Verifica se a exceção DomainLayerException é lançada ao tentar adicionar uma subpasta de tipo inválido.
    /// </summary>
    [Fact]
    public void AddSubFolder_Should_Throw_Exception_For_Invalid_SubFolder_Type()
    {
        // Arrange
        var userFolder = _folderFixture.UserFolder;
        var commonFolder = _folderFixture.CommonFolder;

        // Act & Assert
        Assert.Throws<DomainLayerException>(() => userFolder.AddSubFolder(commonFolder));
    }

    /// <summary>
    /// Verifica se a subpasta é adicionada corretamente.
    /// </summary>
    [Fact]
    public void AddSubFolder_Should_Add_SubFolder_Correctly()
    {
        // Arrange
        var storageFolder = _folderFixture.StorageFolder;
        var commonFolder = new Folder("SubFolder", storageFolder.Owner, storageFolder);
        commonFolder.UpdateType(FolderType.Common);

        // Act
        storageFolder.AddSubFolder(commonFolder);

        // Assert
        Assert.Contains(commonFolder, storageFolder.Folders);
    }

    /// <summary>
    /// Verifica se a exceção DomainLayerException é lançada ao tentar adicionar um arquivo a uma pasta de tipo inválido.
    /// </summary>
    [Fact]
    public void AddFile_Should_Throw_Exception_For_Invalid_Folder_Type()
    {
        // Arrange
        var userFolder = _folderFixture.UserFolder;
        var file = new FileFixture().DefaultFile;

        // Act & Assert
        Assert.Throws<DomainLayerException>(() => userFolder.AddFile(file));
    }

    /// <summary>
    /// Verifica se o arquivo é adicionado corretamente.
    /// </summary>
    [Fact]
    public void AddFile_Should_Add_File_Correctly()
    {
        // Arrange
        var storageFolder = _folderFixture.StorageFolder;
        var file = new FileFixture().DefaultFile;

        // Act
        storageFolder.AddFile(file);

        // Assert
        Assert.Contains(file, storageFolder.Files);
    }
}
