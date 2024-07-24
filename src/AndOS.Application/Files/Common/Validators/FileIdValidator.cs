using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Interfaces;
using FluentValidation;

namespace AndOS.Application.Files.Common.Validators;

public class FileIdValidator : AbstractValidator<Guid>
{
    public FileIdValidator(IReadRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x)
            .NotEmpty().NotNull().NotEqual(Guid.Empty).WithMessage(validationLocalizer["IdEmpty"])
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage(validationLocalizer["InvalidGuid"])
            .MustAsync(async (id, cancellation) => await fileRepository.AnyAsync(new GetFileByIdSpec(id), cancellation)).WithMessage(validationLocalizer["FileNotFound"]);
    }
}