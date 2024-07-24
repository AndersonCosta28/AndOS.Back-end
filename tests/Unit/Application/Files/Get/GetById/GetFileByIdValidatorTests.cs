using AndOS.Application.Files.Get.GetById;

namespace Unit.Application.Files.Get.GetById
{
    public class GetFileByIdValidatorTests
    {
        private readonly GetFileByIdValidator _validator;
        private readonly Mock<IReadRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;

        public GetFileByIdValidatorTests()
        {
            _fileRepositoryMock = new Mock<IReadRepository<File>>();
            _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty"));
            _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID"));
            _validationResourceMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

            _validator = new GetFileByIdValidator(_fileRepositoryMock.Object, _validationResourceMock.Object);
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o Id é inválido.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            Guid model = Guid.Empty;
            TestValidationResult<Guid> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        /// <summary>
        /// Verifica se o validador não retorna erros quando a solicitação é válida.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            Guid model = Guid.NewGuid();

            _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByIdSpec>(), default))
                .ReturnsAsync(true);

            TestValidationResult<Guid> result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x);
        }
    }
}
