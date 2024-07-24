using AndOS.Application.Files.Common.Validators;
using AndOS.Application.Interfaces;
using FluentValidation;

namespace AndOS.Application.Files.Get.GetById;

public class GetFileByIdValidator : AbstractValidator<Guid>
{
    public GetFileByIdValidator(IReadRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x)
            .SetValidator(new FileIdValidator(fileRepository, validationLocalizer));
    }
}