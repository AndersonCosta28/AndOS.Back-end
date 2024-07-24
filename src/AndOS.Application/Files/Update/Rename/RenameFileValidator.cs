using AndOS.Application.Files.Common.Specs;
using AndOS.Application.Files.Common.Validators;
using AndOS.Application.Files.Get.GetById;
using AndOS.Application.Interfaces;
using AndOS.Shared.Requests.Files.Update.Rename;
using FluentValidation;

namespace AndOS.Application.Files.Update.Rename;

public class RenameFileValidator : AbstractValidator<RenameFileRequest>
{
    public string PropertyNameInFileNameAlreadyExistsInDiretory { get; } = "Name/Extension";
    public RenameFileValidator(IRepository<File> fileRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new FileIdValidator(fileRepository, validationLocalizer));

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(validationLocalizer["RequiredFileName"])
            .SetValidator(new FileNameValidator(validationLocalizer));

        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                var file = await fileRepository.FirstOrDefaultAsync(new GetFileByIdSpec(request.Id), cancellation);
                if (file == null)
                {
                    Console.WriteLine($"File not found for Id: {request.Id}");
                    throw new ApplicationLayerException(validationLocalizer["FileNotFound"]);
                }

                var existingFile = await fileRepository.FirstOrDefaultAsync(new GetFileByNameAndParentFolderIdSpec(request.Name, request.Extension, file.ParentFolderId), cancellation);
                var result = existingFile != null && existingFile.Id != request.Id;
                return !result;
            })
            .WithMessage(validationLocalizer["FileNameAlreadyExistsInDirectory"])
            .OverridePropertyName(PropertyNameInFileNameAlreadyExistsInDiretory);
    }
}