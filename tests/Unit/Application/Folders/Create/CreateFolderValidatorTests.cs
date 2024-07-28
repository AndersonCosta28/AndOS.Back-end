using AndOS.Application.Folders.Common.Specs;
using AndOS.Application.Folders.Create;

namespace Unit.Application.Folders.Create
{
    public class CreateFolderValidatorTests
    {
        private readonly CreateFolderValidator _validator;
        private readonly Mock<IRepository<Folder>> _folderRepositoryMock;
        private readonly Mock<IStringLocalizer<ValidationResource>> _localizerMock;

        public CreateFolderValidatorTests()
        {
            _folderRepositoryMock = new Mock<IRepository<Folder>>();
            _localizerMock = new Mock<IStringLocalizer<ValidationResource>>();

            // Setup localization messages
            _localizerMock.Setup(l => l["RequiredFolderName"]).Returns(new LocalizedString("RequiredFolderName", "The folder name is required"));
            _localizerMock.Setup(l => l["FolderNameLength"]).Returns(new LocalizedString("FolderNameLength", "The folder name must be between 1 and 100 characters long"));
            _localizerMock.Setup(l => l["InvalidFolderNameCharacters"]).Returns(new LocalizedString("InvalidFolderNameCharacters", "The folder name cannot contain the characters \\ / : * ? \" < > | and cannot end with a space or a dot"));
            _localizerMock.Setup(l => l["FolderNameAlreadyExistsInDirectory"]).Returns(new LocalizedString("FolderNameAlreadyExistsInDirectory", "A folder with this name already exists in the specified directory"));

            _validator = new CreateFolderValidator(_folderRepositoryMock.Object, _localizerMock.Object);
        }

        /// <summary>
        /// Verifies that the validator returns an error when the folder name is empty.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Empty()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = "", ParentFolderId = null };
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name is required");
        }

        /// <summary>
        /// Verifica se o validador retorna um erro quando o nome da pasta é nulo.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Null()
        {
            // Arrange
            CreateFolderRequest request = new CreateFolderRequest { Name = null, ParentFolderId = Guid.NewGuid() };

            // Act
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage("The folder name is required");
        }

        /// <summary>
        /// Verifies that the validator returns an error when the folder name is too short.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Short()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = "", ParentFolderId = null }; // Less than 1 character
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name must be between 1 and 100 characters long");
        }

        /// <summary>
        /// Verifies that the validator returns an error when the folder name is too long.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Long()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = new string('a', 101), ParentFolderId = null }; // More than 100 characters
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name must be between 1 and 100 characters long");
        }

        /// <summary>
        /// Verifies that the validator returns an error when the folder name contains invalid characters.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Name_Contains_Invalid_Characters()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = "Invalid/Name", ParentFolderId = null }; // Contains '/'
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("The folder name cannot contain the characters \\ / : * ? \" < > | and cannot end with a space or a dot");
        }

        /// <summary>
        /// Verifies that the validator does not return errors when the folder name is valid.
        /// </summary>
        [Fact]
        public async Task Should_Not_Have_Error_When_Name_Is_Valid()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = "ValidName", ParentFolderId = null }; // Valid name
            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        /// <summary>
        /// Verifies that the validator returns an error when a folder with the same name already exists in the specified directory.
        /// </summary>
        [Fact]
        public async Task Should_Have_Error_When_Folder_With_Same_Name_Exists()
        {
            CreateFolderRequest model = new CreateFolderRequest { Name = "ExistingFolder", ParentFolderId = Guid.NewGuid() };
            _folderRepositoryMock.Setup(repo => repo.AnyAsync(It.IsAny<GetFolderByNameAndParentFolderIdSpec>(), default))
                .ReturnsAsync(true);

            TestValidationResult<CreateFolderRequest> result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage("A folder with this name already exists in the specified directory");
        }
    }
}
