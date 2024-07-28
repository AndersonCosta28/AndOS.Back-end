using AndOS.Application.Folders.Common.Specs;
using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Folders.Update.Rename;

namespace Unit.Application.Folders.Update.Rename
{
    public class RenameFolderValidatorTests
    {
        private readonly RenameFolderValidator _validator;
        private readonly Mock<IRepository<Folder>> _folderRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;

        public RenameFolderValidatorTests()
        {
            _folderRepositoryMock = new Mock<IRepository<Folder>>();
            _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty"));
            _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID"));
            _validationResourceMock.Setup(l => l["FolderNotFound"]).Returns(new LocalizedString("FolderNotFound", "Folder not found"));
            _validationResourceMock.Setup(l => l["RequiredFolderName"]).Returns(new LocalizedString("RequiredFolderName", "The folder name is required"));
            _validationResourceMock.Setup(l => l["FolderNameLength"]).Returns(new LocalizedString("FolderNameLength", "The folder name must be between 1 and 100 characters long"));
            _validationResourceMock.Setup(l => l["InvalidFolderNameCharacters"]).Returns(new LocalizedString("InvalidFolderNameCharacters", "The folder name cannot contain the characters \\ / : *? \" < > | and cannot end with a space or a dot"));
            _validationResourceMock.Setup(l => l["FolderNameAlreadyExistsInDirectory"]).Returns(new LocalizedString("FolderNameAlreadyExistsInDirectory", "A folder with this name already exists in the specified directory"));

            _validator = new RenameFolderValidator(_folderRepositoryMock.Object, _validationResourceMock.Object);
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o Id é inválido.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            var model = new RenameFolderRequest { Id = Guid.Empty, Name = "ValidName" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta está vazio.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name is required");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta é nulo.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Null()
        {
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = null };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name is required");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta é muito curto.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "" }; // Menor que 1 caractere
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name must be between 1 and 100 characters long");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta é muito longo.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = new string('a', 101) }; // Mais que 100 caracteres
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name must be between 1 and 100 characters long");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta contém caracteres inválidos.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Contains_Invalid_Characters()
        {
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "Invalid/Name" }; // Contém '/'
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name cannot contain the characters \\ / : *? \" < > | and cannot end with a space or a dot");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando uma pasta com o mesmo nome já existe no diretório especificado.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Folder_With_Same_Name_Exists()
        {
            var oldFolder = new Folder { Id = Guid.NewGuid() };
            oldFolder.UpdateName("ExistingFolder");
            var model = new RenameFolderRequest { Id = Guid.NewGuid(), Name = "ExistingFolder" };

            _folderRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFolderByNameAndParentFolderIdSpec>(), default))
                .ReturnsAsync(oldFolder);

            // Simular que o Id é válido
            _folderRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync(true);

            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("A folder with this name already exists in the specified directory");
        }

        /// <summary>
        /// Verifica se o validador não retorna erros quando a solicitação é válida.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var folderId = Guid.NewGuid();
            var model = new RenameFolderRequest { Id = folderId, Name = "ValidName" };
            _folderRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFolderByIdSpec>(), default))
                .ReturnsAsync(true);
            var result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}