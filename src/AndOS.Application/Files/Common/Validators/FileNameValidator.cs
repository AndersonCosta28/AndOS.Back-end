using AndOS.Core.Schemas;
using FluentValidation;

namespace AndOS.Application.Files.Common.Validators;

public class FileNameValidator : AbstractValidator<string>
{
    public FileNameValidator(IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(name => name)
            .NotEmpty().WithMessage(validationLocalizer["RequiredFileName"])
            .Length(FileSchema.MinLenghtName, FileSchema.MaxLenghtName).WithMessage(validationLocalizer["FileNameLength"])
            .Matches(FileSchema.RegexName).WithMessage(validationLocalizer["InvalidFileNameCharacters"]);
    }
}