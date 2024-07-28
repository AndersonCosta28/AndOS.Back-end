using AndOS.Application.Folders.Delete;
using AndOS.Application.Folders.Get.GetById;

namespace Unit.Application.Folders.Delete
{
    public class DeleteFolderValidationTests
    {
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;
        private readonly DeleteFolderValidation _validator;
        private readonly Mock<IRepository<Folder>> _folderRepositoryMock;

        public DeleteFolderValidationTests()
        {
            _folderRepositoryMock = new Mock<IRepository<Folder>>();
            _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty."));
            _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID."));
            _validationResourceMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found."));

            _validator = new DeleteFolderValidation(_folderRepositoryMock.Object, _validationResourceMock.Object);

        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o Id é inválido.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            DeleteFolderRequest model = new DeleteFolderRequest { Id = Guid.Empty };
            TestValidationResult<DeleteFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        /// <summary>
        /// Verifica se o validador não retorna erros quando a solicitação é válida.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            DeleteFolderRequest model = new DeleteFolderRequest { Id = Guid.NewGuid() };
            _folderRepositoryMock.Setup(f => f.AnyAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync(true);

            TestValidationResult<DeleteFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
