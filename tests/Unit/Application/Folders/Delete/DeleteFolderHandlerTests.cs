﻿using AndOS.Application.Exceptions;
using AndOS.Application.Folders.Delete;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Delete;

namespace Unit.Application.Folders.Delete
{
    public class DeleteFolderHandlerTests
    {
        private readonly Mock<IRepository<Folder>> _folderRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;
        private readonly IRequestHandler<DeleteFolderRequest> _handler;

        public DeleteFolderHandlerTests()
        {
            _folderRepositoryMock = new Mock<IRepository<Folder>>();
            _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Setup localization messages
            _localizerMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found"));

            _handler = new DeleteFolderHandler(
                _folderRepositoryMock.Object,
                _localizerMock.Object);
        }

        /// <summary>
        /// Verifies that the handler deletes the folder when the request is valid.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Delete_Folder_When_Request_Is_Valid()
        {
            // Arrange
            var request = new DeleteFolderRequest { Id = Guid.NewGuid() };
            Folder folder = new Folder("FolderToDelete", new ApplicationUser { Id = Guid.NewGuid() });

            _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync(folder);

            // Act
            await _handler.Handle(request, default);

            // Assert
            _folderRepositoryMock.Verify(f => f.DeleteAsync(folder, default), Times.Once);
        }

        /// <summary>
        /// Verifies that the handler throws an exception when the folder is not found.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Folder_Not_Found()
        {
            // Arrange
            var request = new DeleteFolderRequest { Id = Guid.NewGuid() };

            _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync((Folder)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, default));
            Assert.Equal("Folder not found", exception.Message);
        }
    }
}
