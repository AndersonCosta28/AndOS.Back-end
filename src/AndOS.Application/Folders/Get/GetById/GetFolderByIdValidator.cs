using AndOS.Application.Folders.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Folders.Get.GetById;
using FluentValidation;

namespace AndOS.Application.Folders.Get.GetById;

public class GetFolderByIdValidator : AbstractValidator<GetFolderByIdRequest>
{
    public GetFolderByIdValidator(IReadRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .SetValidator(new FolderIdValidator(folderRepository, validationLocalizer));
    }
}