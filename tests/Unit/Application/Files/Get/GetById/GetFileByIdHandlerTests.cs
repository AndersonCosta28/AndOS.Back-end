using AndOS.Application.Exceptions;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Folders.Get.GetAccountFolderInParentFolder;
using Common.Fixtures;

namespace Unit.Application.Files.Get.GetById
{
    public class GetFileByIdHandlerTests : IClassFixture<FileFixture>
    {
        private readonly Mock<IRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationLocalizerMock;
        private readonly Mock<ICloudStorageServiceFactory> _cloudStorageServiceFactoryMock;
        private readonly Mock<IMapperService> _mapperServiceMock;
        private readonly Mock<ISender> _senderMock;
        private readonly GetFileByIdHandler _handler;
        private readonly FileFixture _fileFixture;

        public GetFileByIdHandlerTests(FileFixture fileFixture)
        {

            _fileRepositoryMock = new Mock<IRepository<File>>();
            _validationLocalizerMock = new Mock<IStringLocalizer<ValidationResource>>();
            _cloudStorageServiceFactoryMock = new Mock<ICloudStorageServiceFactory>();
            _mapperServiceMock = new Mock<IMapperService>();
            _senderMock = new Mock<ISender>();

            _handler = new GetFileByIdHandler(
                _fileRepositoryMock.Object,
                _cloudStorageServiceFactoryMock.Object,
                _validationLocalizerMock.Object,
                _mapperServiceMock.Object,
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
            GetFileByIdRequest request = new(Guid.NewGuid());
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((File)null);

            _validationLocalizerMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

            // Act & Assert
            ApplicationLayerException exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Equal("File not found", exception.Message);
        }

        /// <summary>
        /// Verifica se o manipulador retorna a URL de download corretamente.
        /// </summary>
        [Fact]
        public async Task Should_Return_Download_Url_Successfully()
        {
            // Arrange
            string expectedUrl = "https://example.com/download";
            File file = _fileFixture.DefaultFile;
            GetFileByIdRequest request = new(file.Id);
            GetFileByIdResponse response = new()
            {
                Id = file.Id,
                Url = expectedUrl
            };
            var account = file.ParentFolder.GetAccount();

            _mapperServiceMock.Setup(mapperService => mapperService.MapAsync<GetFileByIdResponse>(file)).ReturnsAsync(response);
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(file);

            _senderMock.Setup(repo => repo.Send(new GetAccountFolderInParentFolderRequest(file.ParentFolder.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(account.Folder);

            Mock<ICloudStorageService> cloudStorageServiceMock = new Mock<ICloudStorageService>();
            _cloudStorageServiceFactoryMock.Setup(f => f.GetCloudStorageService(account.CloudStorage)).Returns(cloudStorageServiceMock.Object);
            cloudStorageServiceMock.Setup(c => c.GetUrlDownloadFileAsync(file, account)).ReturnsAsync(expectedUrl);

            // Act
            GetFileByIdResponse result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedUrl, result.Url);
            Assert.Equal(response, result);
        }
    }
}
