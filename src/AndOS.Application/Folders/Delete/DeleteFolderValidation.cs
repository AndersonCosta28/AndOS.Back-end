using AndOS.Application.Folders.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Delete;
using FluentValidation;

namespace AndOS.Application.Folders.Delete;

public class DeleteFolderValidation : AbstractValidator<DeleteFolderRequest>
{
    public DeleteFolderValidation(IRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .SetValidator(new FolderIdValidator(folderRepository, validationLocalizer));
    }
}