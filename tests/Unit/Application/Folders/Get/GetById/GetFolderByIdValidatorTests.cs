using AndOS.Application.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetById;

namespace Unit.Application.Folders.Get.GetById
{
    public class GetFolderByIdValidatorTests
    {
        private readonly GetFolderByIdValidator _validator;
        private readonly Mock<IReadRepository<Folder>> _folderRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;

        public GetFolderByIdValidatorTests()
        {
            _folderRepositoryMock = new Mock<IReadRepository<Folder>>();
            _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty."));
            _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID."));
            _validationResourceMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found."));

            _validator = new GetFolderByIdValidator(_folderRepositoryMock.Object, _validationResourceMock.Object);
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o Id é inválido.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            GetFolderByIdRequest model = new GetFolderByIdRequest { Id = Guid.Empty };
            TestValidationResult<GetFolderByIdRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        /// <summary>
        /// Verifica se o validador não retorna erros quando a solicitação é válida.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            // Arrange
            GetFolderByIdRequest model = new GetFolderByIdRequest { Id = Guid.NewGuid() };
            // Simular que a pasta existe no repositório
            _folderRepositoryMock.Setup(f => f.AnyAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync(true);

            // Act
            TestValidationResult<GetFolderByIdRequest> result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
