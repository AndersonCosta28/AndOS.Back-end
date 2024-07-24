using AndOS.Core.Schemas;
using FluentValidation;

namespace AndOS.Application.Folders.Common.Validators;

public class FolderNameValidator : AbstractValidator<string>
{
    public FolderNameValidator(IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(name => name)
            .NotEmpty().WithMessage(validationLocalizer["RequiredFolderName"])
            .Length(FolderSchema.MinLenghtName, FolderSchema.MaxLenghtName).WithMessage(validationLocalizer["FolderNameLength"])
            .Matches(FolderSchema.RegexName).WithMessage(validationLocalizer["InvalidFolderNameCharacters"]);
    }
}