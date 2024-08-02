using AndOS.Application.Exceptions;
using AndOS.Application.Folders.Create;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Create;

namespace Unit.Application.Folders.Create;

public class CreateFolderHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapperService> _mapperMock;
    private readonly Mock<IRepository<Folder>> _folderRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly IRequestHandler<CreateFolderRequest, CreateFolderResponse> _handler;
    private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;

    public CreateFolderHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapperService>();
        _folderRepositoryMock = new Mock<IRepository<Folder>>();
        _currentUserContextMock = new Mock<ICurrentUserContext>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

        // Setup localization messages
        _localizerMock.Setup(l => l["ParentFolderNotFound"]).Returns(new LocalizedString("ParentFolderNotFound", "The parent folder was not found."));


        _handler = new CreateFolderHandler(
            _unitOfWorkMock.Object,
            _folderRepositoryMock.Object,
            _currentUserContextMock.Object,
            _authorizationServiceMock.Object,
            _localizerMock.Object);
    }

    /// <summary>
    /// Verifica se o handler cria uma nova pasta quando a solicitação é válida.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Create_Folder_When_Request_Is_Valid()
    {
        // Arrange
        var request = new CreateFolderRequest { Name = "NewFolder", ParentFolderId = null };
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var folder = new Folder(request.Name, user, null);
        var response = new CreateFolderResponse(Guid.NewGuid());
        _currentUserContextMock.Setup(c => c.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _folderRepositoryMock.Setup(f => f.AddAsync(folder, default)).ReturnsAsync(folder);
        _unitOfWorkMock.Setup(u => u.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<CreateFolderResponse>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Callback((Func<CancellationToken, Task<CreateFolderResponse>> operation, CancellationToken cancellationToken) =>
            {
                var result = operation.Invoke(cancellationToken);
            });

        // Act
        var result = await _handler.Handle(request, default);

        // Assert
        Assert.IsType<CreateFolderResponse>(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    /// <summary>
    /// Verifica se o handler lança uma exceção quando a pasta pai não é encontrada.
    /// </summary>
    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Parent_Folder_Not_Found()
    {
        // Arrange
        var request = new CreateFolderRequest { Name = "NewFolder", ParentFolderId = Guid.NewGuid() };
        var user = new ApplicationUser { Id = Guid.NewGuid() };

        _currentUserContextMock.Setup(c => c.GetCurrentUserAsync(It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default)).ReturnsAsync((Folder)null);
        var response = new CreateFolderResponse(Guid.NewGuid());

        _unitOfWorkMock.Setup(u => u.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<CreateFolderResponse>>>(), It.IsAny<CancellationToken>()))
            .Callback((Func<CancellationToken, Task<CreateFolderResponse>> operation, CancellationToken cancellationToken) =>
            {
                operation.Invoke(cancellationToken);
                throw new ApplicationLayerException(_localizerMock.Object["ParentFolderNotFound"]);
            });
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, default));
        Assert.Equal("The parent folder was not found.", exception.Message);
    }
}
