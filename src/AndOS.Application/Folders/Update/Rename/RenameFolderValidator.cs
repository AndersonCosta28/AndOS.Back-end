using AndOS.Application.Folders.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Update.Rename;
using FluentValidation;

namespace AndOS.Application.Folders.Update.Rename;

public class RenameFolderValidator : AbstractValidator<RenameFolderRequest>
{
    public RenameFolderValidator(IRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .SetValidator(new FolderIdValidator(folderRepository, validationLocalizer));

        RuleFor(x => x.Name)
            .NotNull().WithMessage(validationLocalizer["RequiredFolderName"])
            .SetValidator(new FolderNameValidator(validationLocalizer));

        RuleFor(x => new FolderNameDuplicateSameFolderValidatorParams(x.Id, x.Name, null))
            .SetValidator(new FolderNameDuplicateSameFolderValidator(folderRepository, validationLocalizer));
    }
}