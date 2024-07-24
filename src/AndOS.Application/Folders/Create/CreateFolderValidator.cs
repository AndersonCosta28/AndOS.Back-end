using AndOS.Application.Folders.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Create;
using FluentValidation;

namespace AndOS.Application.Folders.Create;

public class CreateFolderValidator : AbstractValidator<CreateFolderRequest>
{
    public CreateFolderValidator(IRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage(validationLocalizer["RequiredFolderName"])
            .SetValidator(new FolderNameValidator(validationLocalizer));

        RuleFor(x => new FolderNameDuplicateSameFolderValidatorParams(x.Name, x.ParentFolderId))
            .SetValidator(new FolderNameDuplicateSameFolderValidator(folderRepository, validationLocalizer));
    }
}