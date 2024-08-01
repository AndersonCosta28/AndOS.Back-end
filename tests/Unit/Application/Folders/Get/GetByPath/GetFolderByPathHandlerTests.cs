using AndOS.Application.Exceptions;
using AndOS.Application.Folders.Common.Specs;
using AndOS.Application.Folders.Get.GetByPath;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;

namespace Unit.Application.Folders.Get.GetByPath
{
    public class GetFolderByPathHandlerTests
    {
        private readonly Mock<IReadRepository<Folder>> _folderRepositoryMock;
        private readonly Mock<ISender> _senderMock;
        private readonly Mock<IMapperService> _mapperServiceMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;
        private readonly IRequestHandler<GetFolderByPathRequest, GetFolderByPathResponse> _handler;

        public GetFolderByPathHandlerTests()
        {
            _folderRepositoryMock = new Mock<IReadRepository<Folder>>();
            _senderMock = new Mock<ISender>();
            _mapperServiceMock = new Mock<IMapperService>();
            _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Setup localization messages
            _localizerMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found"));

            _handler = new GetFolderByPathHandler(
                _folderRepositoryMock.Object,
                _senderMock.Object,
                _mapperServiceMock.Object,
                _localizerMock.Object);
        }

        /// <summary>
        /// Verifica se o handler retorna as pastas na raiz quando o caminho é vazio ou nulo.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Return_Folders_In_Root_When_Path_Is_Empty()
        {
            // Arrange
            var request = new GetFolderByPathRequest(string.Empty);
            var foldersInRoot = new List<ChildrenFolderDTO>
            {
                new () { Id = Guid.NewGuid(), Name = "RootFolder1" },
                new (){ Id = Guid.NewGuid(), Name = "RootFolder2" }
            };

            _folderRepositoryMock.Setup(f => f.ProjectToListAsync<ChildrenFolderDTO>(It.IsAny<GetFolderByParentFolderIsNullSpec>(), default))
                .ReturnsAsync(foldersInRoot);

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(foldersInRoot, result.Folders);
            Assert.Empty(result.CurrentPath);
            Assert.Empty(result.Files);
            Assert.Empty(result.FullPath);
            Assert.Empty(result.Name);
            Assert.Null(result.ParentFolder);
            Assert.Null(result.Icon);
            Assert.Contains(FolderPermission.Read, result.Permissions);
        }

        /// <summary>
        /// Verifica se o handler retorna a pasta correspondente quando o caminho é válido.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Return_Folder_When_Path_Is_Valid()
        {
            // Arrange
            var folderId = Guid.NewGuid();
            var request = new GetFolderByPathRequest("ValidPath");
            var folder = new Folder("ValidPath", new ApplicationUser { Id = Guid.NewGuid() }) { Id = folderId };
            var folderResponse = new GetFolderByIdResponse
            {
                Id = folderId,
                Name = "ValidPath",
                Files = [],
                Folders = [],
                Permissions = [FolderPermission.Read]
            };

            _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByNameAndParentFolderIdSpec>(), default))
                .ReturnsAsync(folder);

            _senderMock.Setup(s => s.Send(It.IsAny<GetFolderByIdRequest>(), default))
                .ReturnsAsync(folderResponse);

            _mapperServiceMock.Setup(m => m.MapAsync<GetFolderByPathResponse>(It.IsAny<GetFolderByIdResponse>()))
                .ReturnsAsync(new GetFolderByPathResponse
                {
                    Id = folderId,
                    Name = "ValidPath",
                    Files = [],
                    Folders = [],
                    Permissions = [FolderPermission.Read]
                });

            // Act
            GetFolderByPathResponse result = await _handler.Handle(request, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(folderId, result.Id);
            Assert.Equal("ValidPath", result.Name);
        }

        /// <summary>
        /// Verifica se o handler lança uma exceção quando o caminho é inválido.
        /// </summary>
        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Path_Is_Invalid()
        {
            // Arrange
            var request = new GetFolderByPathRequest("InvalidPath");

            _folderRepositoryMock.Setup(f => f.FirstOrDefaultAsync(It.IsAny<GetFolderByNameAndParentFolderIdSpec>(), default))
                .ReturnsAsync((Folder)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationLayerException>(() => _handler.Handle(request, default));
            Assert.Equal("Folder not found", exception.Message);
        }
    }
}
