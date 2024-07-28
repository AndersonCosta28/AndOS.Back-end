using AndOS.Application.Exceptions;
using AndOS.Application.Files.Common.Specs;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Files.Update.Rename;
using Common.Fixtures;

namespace Unit.Application.Files.Update.Rename
{
    public class RenameFileValidatorTests : IClassFixture<FileFixture>
    {
        private readonly RenameFileValidator _validator;
        private readonly Mock<IRepository<File>> _fileRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _validationResourceMock;
        private readonly FileFixture _fileFixture;

        public RenameFileValidatorTests(FileFixture fileFixture)
        {
            _fileRepositoryMock = new Mock<IRepository<File>>();
            _validationResourceMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Configurar os mocks para retornar valores válidos
            _validationResourceMock.Setup(l => l["IdEmpty"]).Returns(new LocalizedString("IdEmpty", "The identifier cannot be empty"));
            _validationResourceMock.Setup(l => l["InvalidGuid"]).Returns(new LocalizedString("InvalidGuid", "The identifier must be a valid GUID"));
            _validationResourceMock.Setup(l => l["FileNotFound"]).Returns(new LocalizedString("FileNotFound", "File not found"));
            _validationResourceMock.Setup(l => l["RequiredFileName"]).Returns(new LocalizedString("RequiredFileName", "The file name is required"));
            _validationResourceMock.Setup(l => l["FileNameLength"]).Returns(new LocalizedString("FileNameLength", "The file name must be between 1 and 100 characters long"));
            _validationResourceMock.Setup(l => l["InvalidFileNameCharacters"]).Returns(new LocalizedString("InvalidFileNameCharacters", "The file name cannot contain the characters \\ / : *? \" < > | and cannot end with a space or a dot"));
            _validationResourceMock.Setup(l => l["FileNameAlreadyExistsInDirectory"]).Returns(new LocalizedString("FileNameAlreadyExistsInDirectory", "A file with this name already exists in the specified directory"));

            _validator = new RenameFileValidator(_fileRepositoryMock.Object, _validationResourceMock.Object);

            // Configurar o mock do repositório para retornar um arquivo válido por padrão
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), default))
                .ReturnsAsync(new File { Id = Guid.NewGuid() });
            _fileFixture = fileFixture;
        }

        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Invalid()
        {
            var model = new RenameFileRequest { Id = Guid.Empty, Name = "ValidName" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = "" };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The file name is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Null()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = null };
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The file name is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Short()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = "" }; // Menor que 1 caractere
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The file name must be between 1 and 100 characters long");
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Long()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = new string('a', 101) }; // Mais que 100 caracteres
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The file name must be between 1 and 100 characters long");
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Contains_Invalid_Characters()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = "Invalid/Name" }; // Contém '/'
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The file name cannot contain the characters \\ / : *? \" < > | and cannot end with a space or a dot");
        }

        [Fact]
        public async Task Should_Have_Error_When_File_With_Same_Name_Exists()
        {
            var oldFile = new File { Id = Guid.NewGuid() };
            oldFile.UpdateName("ExistingFile");
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = "ExistingFile" };

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByNameAndParentFolderIdSpec>(), default))
                .ReturnsAsync(oldFile);

            // Simular que o Id é válido
            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), default))
                .ReturnsAsync(new File { Id = model.Id });

            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(_validator.PropertyNameInFileNameAlreadyExistsInDiretory)
                  .WithErrorMessage("A file with this name already exists in the specified directory");
        }

        [Fact]
        public async Task Should_Have_Error_When_File_Not_Found()
        {
            var model = new RenameFileRequest { Id = Guid.NewGuid(), Name = "ValidName" };

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), default))
                .ReturnsAsync((File)null);

            var exception = await Assert.ThrowsAsync<ApplicationLayerException>(async () =>
            {
                await _validator.TestValidateAsync(model);
            });

            Assert.Equal("File not found", exception.Message);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Request_Is_Valid()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var folderExisting = _fileFixture.DefaultFile;
            folderExisting.Id = fileId;
            var model = new RenameFileRequest { Id = fileId, Name = "ValidName", Extension = "txt" };

            _fileRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFileByIdSpec>(), default))
                    .ReturnsAsync(true);

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByIdSpec>(), default))
                    .ReturnsAsync(folderExisting);

            _fileRepositoryMock.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<GetFileByNameAndParentFolderIdSpec>(), default))
                    .ReturnsAsync(folderExisting);
            // Act
            var result = await _validator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }
    }
}