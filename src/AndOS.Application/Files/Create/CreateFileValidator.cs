using AndOS.Application.Files.Common.Specs;
using AndOS.Application.Files.Common.Validators;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Create;
using FluentValidation;
namespace AndOS.Application.Files.Create;

public class CreateFileValidator : AbstractValidator<CreateFileRequest>
{
    public CreateFileValidator(IReadRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage(validationLocalizer["RequiredFileName"])
            .SetValidator(new FileNameValidator(validationLocalizer));

        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                bool result = await fileRepository.AnyAsync(new GetFileByNameAndParentFolderIdSpec(request.Name, request.Extension, request.ParentFolderId), cancellation);
                return !result;
            })
            .WithMessage(validationLocalizer["FileNameAlreadyExistsInDirectory"])
            .OverridePropertyName("Name/Extension");
    }
}