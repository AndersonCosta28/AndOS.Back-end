using AndOS.Application.Exceptions;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Files.Update.Rename;
using AndOS.Shared.Requests.Files.Update.Rename;
using Common.Fixtures;

namespace Unit.Application.Files.Update.Rename
{
    public class RenameFileHandlerTests : IClassFixture<FileFixture>
    {
        private readonly Mock<IRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationLocalizerMock;
        private readonly RenameFileHandler _handler;
        private readonly FileFixture _fileFixture;

        public RenameFileHandlerTests(FileFixture fileFixture)
        {
            _fileRepositoryMock = new Mock<IRepository<File>>();
            _validationLocalizerMock = new Mock<IStringLocalizer<ValidationResource>>();

            _handler = new RenameFileHandler(
                _fileRepositoryMock.Object,
                _validationLocalizerMock.Object
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
            RenameFileRequest request = new RenameFileRequest { Id = Guid.NewGuid(), Name = "NewName", Extension = "txt" };
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((File)null);

            _validationLocalizerMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

            // Act & Assert
            ApplicationLayerException exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("File not found", exception.Message);
        }

        /// <summary>
        /// Verifica se o manipulador renomeia o arquivo corretamente.
        /// </summary>
        [Fact]
        public async Task Should_Rename_File_Successfully()
        {
            // Arrange
            var file = _fileFixture.DefaultFile;

            RenameFileRequest request = new RenameFileRequest { Id = file.Id, Name = "NewName", Extension = "md" };

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(file);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _fileRepositoryMock.Verify(repo => repo.UpdateAsync(file, It.IsAny<CancellationToken>()), Times.Once);
            //_unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("NewName", file.Name);
            Assert.Equal("md", file.Extension);
        }
    }
}
