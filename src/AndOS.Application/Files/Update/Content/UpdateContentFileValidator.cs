using AndOS.Application.Files.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Update.Content;
using FluentValidation;
namespace AndOS.Application.Files.Update.Content;

public class UploadFileValidator : AbstractValidator<UpdateContentFileRequest>
{
    public UploadFileValidator(IReadRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .SetValidator(new FileIdValidator(fileRepository, validationLocalizer));
    }
}