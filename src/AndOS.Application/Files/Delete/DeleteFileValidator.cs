using AndOS.Application.Files.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Delete;
using FluentValidation;

namespace AndOS.Application.Files.Delete;

public class DeleteFileValidation : AbstractValidator<DeleteFileRequest>
{
    public DeleteFileValidation(IReadRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .SetValidator(new FileIdValidator(fileRepository, validationLocalizer));
    }
}