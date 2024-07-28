using AndOS.Application.Exceptions;
using AndOS.Application.Files.Delete;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using Common.Fixtures;

namespace Unit.Application.Files.Delete
{
    public class DeleteFileHandlerTests : IClassFixture<FileFixture>
    {
        private readonly Mock<IRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationLocalizerMock;
        private readonly Mock<ICloudStorageServiceFactory> _cloudStorageServiceFactoryMock;
        private readonly Mock<ICloudStorageService> _cloudStorageServiceMock;
        private readonly Mock<ISender> _senderMock;
        private readonly DeleteFileHandler _handler;
        private readonly FileFixture _fileFixture;

        public DeleteFileHandlerTests(FileFixture fileFixture)
        {
            _fileRepositoryMock = new Mock<IRepository<File>>();
            _validationLocalizerMock = new Mock<IStringLocalizer<ValidationResource>>();
            _cloudStorageServiceFactoryMock = new Mock<ICloudStorageServiceFactory>();
            _cloudStorageServiceMock = new Mock<ICloudStorageService>();

            _cloudStorageServiceFactoryMock.Setup(factory => factory.GetCloudStorageService(It.IsAny<CloudStorage>()))
                .Returns(_cloudStorageServiceMock.Object);
            _senderMock = new Mock<ISender>();

            _handler = new DeleteFileHandler(
                _fileRepositoryMock.Object,
                _validationLocalizerMock.Object,
                _cloudStorageServiceFactoryMock.Object,
                _senderMock.Object
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
            DeleteFileRequest request = new DeleteFileRequest { Id = Guid.NewGuid() };
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((File)null);

            _validationLocalizerMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

            // Act & Assert
            ApplicationLayerException exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("File not found", exception.Message);
        }

        /// <summary>
        /// Verifica se o manipulador deleta o arquivo corretamente.
        /// </summary>
        [Fact]
        public async Task Should_Delete_File_Successfully()
        {
            // Arrange
            var file = _fileFixture.DefaultFile;
            var request = new DeleteFileRequest { Id = file.Id };
            Account account = file.ParentFolder.GetAccount();

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(file);

            _senderMock.Setup(repo => repo.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), It.IsAny<CancellationToken>()))
               .ReturnsAsync(account.Folder);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _fileRepositoryMock.Verify(repo => repo.DeleteAsync(file, It.IsAny<CancellationToken>()), Times.Once);
            _cloudStorageServiceMock.Verify(service => service.DeleteFileAsync(file, account), Times.Once);
        }

    }
}
