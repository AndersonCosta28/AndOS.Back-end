using AndOS.Application.Accounts.Get.GetById;
using AndOS.Application.Exceptions;
using AndOS.Application.Files.Create;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Core.Enums;
using AndOS.Shared.Requests.Files.Create;
using Common.Fixtures;
using NuGet.Protocol.Plugins;
using System.Threading;
using ISender = MediatR.ISender;


namespace Unit.Application.Files.Create;

public class CreateFileHandlerTests : IClassFixture<FileFixture>, IClassFixture<FolderFixture>
{
    private readonly Mock<IMapperService> _mapperMock;
    private readonly Mock<IRepository<File>> _fileRepositoryMock;
    private readonly Mock<IReadRepository<Folder>> _folderRepositoryMock;
    private readonly Mock<IReadRepository<Account>> _accountReadRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<ICloudStorageServiceFactory> _cloudStorageServiceFactoryMock;
    private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly Mock<ISender> _senderMock;
    private readonly IRequestHandler<CreateFileRequest, CreateFileResponse> _handler;
    private readonly FileFixture _fileFixture;
    private readonly FolderFixture _folderFixture;

    public CreateFileHandlerTests(FileFixture fileFixture, FolderFixture folderFixture)
    {
        _mapperMock = new Mock<IMapperService>();
        _fileRepositoryMock = new Mock<IRepository<File>>();
        _folderRepositoryMock = new Mock<IReadRepository<Folder>>();
        _accountReadRepositoryMock = new Mock<IReadRepository<Account>>();
        _currentUserContextMock = new Mock<ICurrentUserContext>();
        _cloudStorageServiceFactoryMock = new Mock<ICloudStorageServiceFactory>();
        _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _senderMock = new Mock<ISender>();

        // Setup localization messages
        _localizerMock.Setup(l => l["AccountNotFound"]).Returns(new LocalizedString("AccountNotFound", "Account not found"));
        _localizerMock.Setup(l => l["ParentFolderNotFound"]).Returns(new LocalizedString("ParentFolderNotFound", "Parent folder not found"));
        _handler = new CreateFileHandler(            
            _fileRepositoryMock.Object,
            _folderRepositoryMock.Object,
            _currentUserContextMock.Object,
            _localizerMock.Object,
            _cloudStorageServiceFactoryMock.Object,
            _authorizationServiceMock.Object,
            _senderMock.Object
        );
        _fileFixture = fileFixture;
        _folderFixture = folderFixture;
    }

    /// <summary>
    /// Verifica se o handler cria um novo arquivo quando a solicitação é válida.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Create_File_When_Request_Is_Valid()
    {
        // Arrange
        string expectedUrl = "https://example.com/download";

        var file = _fileFixture.DefaultFile;
        var request = new CreateFileRequest(file.Name, file.Extension, file.Size, file.ParentFolder.Id);
        var account = file.ParentFolder.GetAccount();

        _senderMock.Setup(repo => repo.Send(It.IsAny<GetAccountFolderInParentFolderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((account.Folder));

        _currentUserContextMock.Setup(c => c.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(file.GetUser());
        _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(file.ParentFolder);
        _fileRepositoryMock.Setup(f => f.AddAsync(It.IsAny<File>(), It.IsAny<CancellationToken>())).ReturnsAsync(file);

        Mock<ICloudStorageService> cloudStorageServiceMock = new Mock<ICloudStorageService>();
        _cloudStorageServiceFactoryMock.Setup(f => f.GetCloudStorageService(account.CloudStorage)).Returns(cloudStorageServiceMock.Object);
        cloudStorageServiceMock.Setup(c => c.GetUploadUrlAsync(It.IsAny<File>(), It.IsAny<Account>())).ReturnsAsync(expectedUrl);

        // Act
        var response = await _handler.Handle(request, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotNull(response.Url);
        Assert.Equal(expectedUrl, response.Url);
        _fileRepositoryMock.Verify(f => f.AddAsync(It.IsAny<File>(), default), Times.Once);
        //_unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
    }

    /// <summary>
    /// Verifica se o handler lança uma exceção quando a pasta pai não é encontrada.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Parent_Folder_Not_Found()
    {
        // Arrange            
        var request = new CreateFileRequest("NewFile", "txt", "10MB", Guid.NewGuid());
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var account = new Account { Id = Guid.NewGuid() };
        account.UpdateCloudStorage(CloudStorage.Azure_BlobStorage);

        _currentUserContextMock.Setup(c => c.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _accountReadRepositoryMock.Setup(a => a.FirstOrDefaultAsync(It.IsAny<GetAccountByIdSpec>(), default)).ReturnsAsync(account);
        _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default)).ReturnsAsync((Folder)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, default));
        Assert.Equal("Parent folder not found", exception.Message);
        //_unitOfWorkMock.Verify(u => u.RollbackAsync(default), Times.Once);
    }
}
