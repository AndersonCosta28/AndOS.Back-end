using AndOS.Application.Files.Delete;
using AndOS.Application.Files.Get.GetById;

namespace Unit.Application.Files.Delete
{
    public class DeleteFileValidationTests
    {
        private readonly DeleteFileValidation _validator;
        private readonly Mock<IReadRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationLocalizerMock;

        public DeleteFileValidationTests()
        {
            _fileRepositoryMock = new Mock<IReadRepository<File>>();
            _validationLocalizerMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationLocalizerMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty"));
            _validationLocalizerMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID"));
            _validationLocalizerMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

            _validator = new DeleteFileValidation(_fileRepositoryMock.Object, _validationLocalizerMock.Object);
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o Id é inválido.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            DeleteFileRequest model = new DeleteFileRequest { Id = Guid.Empty };
            TestValidationResult<DeleteFileRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        /// <summary>
        /// Verifica se o validador não retorna erros quando a solicitação é válida.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            DeleteFileRequest model = new DeleteFileRequest { Id = Guid.NewGuid() };
            _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByIdSpec>(), default))
                .ReturnsAsync(true);
            TestValidationResult<DeleteFileRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
