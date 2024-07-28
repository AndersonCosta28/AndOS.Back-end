using AndOS.Application.Exceptions;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Files.Update.Content;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using Common.Fixtures;

namespace Unit.Application.Files.Update.UpdateContentValidatorTests;

public class UpdateContentHandlerTests : IClassFixture<FileFixture>
{
    private readonly Mock<IRepository<File>> _fileRepositoryMock;
    private readonly Mock<IStringLocalizer<ValidationResource>> _validationLocalizerMock;
    private readonly Mock<ICloudStorageServiceFactory> _cloudStorageServiceFactoryMock;
    private readonly UploadFileHandler _handler;
    private readonly FileFixture _fileFixture;
    private readonly Mock<ISender> _senderMock;

    public UpdateContentHandlerTests(FileFixture fileFixture)
    {
        _fileRepositoryMock = new Mock<IRepository<File>>();
        _validationLocalizerMock = new Mock<IStringLocalizer<ValidationResource>>();
        _senderMock = new Mock<ISender>();

        _cloudStorageServiceFactoryMock = new Mock<ICloudStorageServiceFactory>();

        _handler = new UploadFileHandler(
            _senderMock.Object,
            _fileRepositoryMock.Object,
            _validationLocalizerMock.Object,
            _cloudStorageServiceFactoryMock.Object
        );
        _fileFixture = fileFixture;
    }

    /// <summary>
    /// Verifica se o manipulador lança uma exceção quando o arquivo não é encontrado.
    /// </summary>
    [Fact]
    public async Task Should_Throw_Exception_When_File_Not_Found()
    {
        // Arrange
        var request = new UpdateContentFileRequest(Guid.NewGuid());
        _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((File)null);

        _validationLocalizerMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.Equal("File not found", exception.Message);
    }

    /// <summary>
    /// Verifica se o manipulador retorna a URL de upload corretamente.
    /// </summary>
    [Fact]
    public async Task Should_Return_Upload_Url_Successfully()
    {
        // Arrange        
        File file = _fileFixture.DefaultFile;
        var account = file.ParentFolder.GetAccount();
        var request = new UpdateContentFileRequest(file.Id);
        string expectedUrl = "https://example.com/upload";

        _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(file);

        _senderMock.Setup(repo => repo.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account.Folder);

        Mock<ICloudStorageService> cloudStorageServiceMock = new();

        cloudStorageServiceMock.Setup(c => c.GetUploadUrlAsync(file, account)).ReturnsAsync(expectedUrl);

        _cloudStorageServiceFactoryMock.Setup(f => f.GetCloudStorageService(account.CloudStorage)).Returns(cloudStorageServiceMock.Object);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedUrl, response.Url);
        //_unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
