using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Files.Update.Content;
using AndOS.Shared.Requests.Files.Update.Content;

namespace Unit.Application.Files.Update.UpdateContentValidatorTests;

public class UpdateContentValidatorTests
{
    private readonly UploadFileValidator _validator;
    private readonly Mock<IReadRepository<File>> _fileRepositoryMock;
    private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;

    public UpdateContentValidatorTests()
    {
        _fileRepositoryMock = new Mock<IReadRepository<File>>();
        _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

        // Configurar os mocks para retornar valores válidos
        _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty"));
        _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID"));
        _validationResourceMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));

        _validator = new UploadFileValidator(_fileRepositoryMock.Object, _validationResourceMock.Object);
    }

    /// <summary>
    /// Verifica se o validador retorna um erro quando o Id é inválido.
    /// </summary>
    [Fact]
    public async Task Should_Have_Error_When_Id_Is_Invalid()
    {
        var model = new UpdateContentFileRequest(Guid.Empty);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    /// <summary>
    /// Verifica se o validador não retorna erros quando a solicitação é válida.
    /// </summary>
    [Fact]
    public async Task Should_Not_Have_Error_When_Request_Is_Valid()
    {
        var model = new UpdateContentFileRequest(Guid.NewGuid());
        _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByIdSpec>(), default))
            .ReturnsAsync(true);
        var result = await _validator.TestValidateAsync(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
