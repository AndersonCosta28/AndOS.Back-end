using AndOS.Application.Folders.Get.GetById;
using AndOS.Application.Interfaces;
using FluentValidation;

namespace AndOS.Application.Folders.Common.Validators;

public class FolderIdValidator : AbstractValidator<Guid>
{
    public FolderIdValidator(IReadRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x)
            .NotEmpty().NotNull().NotEqual(Guid.Empty).WithMessage(validationLocalizer["IdEmpty"])
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage(validationLocalizer["InvalidGuid"])
            .MustAsync(async (id, cancellation) =>
            {
                bool result = await folderRepository.AnyAsync(new GetFolderByIdSpec(id), cancellation);
                return result;
            }).WithMessage(validationLocalizer["FolderNotFound"]);
    }
}