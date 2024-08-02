using AndOS.Application.Exceptions;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Folders.Update.Rename;
using AndOS.Shared.Requests.Folders.Update.Rename;

namespace Unit.Application.Folders.Update.Rename;

public class RenameFolderHandlerTests
{
    private readonly Mock<IRepository<Folder>> _folderRepositoryMock;
    private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;
    private readonly IRequestHandler<RenameFolderRequest> _handler;

    public RenameFolderHandlerTests()
    {
        _folderRepositoryMock = new Mock<IRepository<Folder>>();
        _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

        // Setup localization messages
        _localizerMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found"));

        _handler = new RenameFolderHandler(
            _folderRepositoryMock.Object,
            _localizerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Rename_Folder_When_Request_Is_Valid()
    {
        // Arrange
        var request = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "NewFolderName" };
        var folder = new Folder("OldFolderName", new ApplicationUser { Id = Guid.NewGuid() });

        _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default))
            .ReturnsAsync(folder);

        // Act
        await _handler.Handle(request, default);

        // Assert
        _folderRepositoryMock.Verify(f => f.UpdateAsync(folder, default), Times.Once);
        //_unitOfWorkMock.Verify(u => u.CommitAsync(default), Times.Once);
        Assert.Equal("NewFolderName", folder.Name);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Folder_Not_Found()
    {
        // Arrange
        var request = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "NewFolderName" };

        _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default))
            .ReturnsAsync((Folder)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, default));
        Assert.Equal("Folder not found", exception.Message);
        //_unitOfWorkMock.Verify(u => u.RollbackAsync(default), Times.Once);
    }
}
