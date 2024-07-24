using AndOS.Application.Folders.Common.Specs;
using AndOS.Application.Interfaces;
using FluentValidation;

namespace AndOS.Application.Folders.Common.Validators;

public struct FolderNameDuplicateSameFolderValidatorParams
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentFolderId { get; set; }
    public FolderNameDuplicateSameFolderValidatorParams(string name, Guid? parentFolderId) : this()
    {
        Name = name;
        ParentFolderId = parentFolderId;
    }

    public FolderNameDuplicateSameFolderValidatorParams(Guid id, string name, Guid? parentFolderId) : this()
    {
        Id = id;
        Name = name;
        ParentFolderId = parentFolderId;
    }
}

public class FolderNameDuplicateSameFolderValidator : AbstractValidator<FolderNameDuplicateSameFolderValidatorParams>
{
    public FolderNameDuplicateSameFolderValidator(IReadRepository<Folder> folderRepository, IStringLocalizer<ValidationResource> validationLocalizer)
    {
        // Atualização
        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                Folder existingFolder = await folderRepository.FirstOrDefaultAsync(new GetFolderByNameAndParentFolderIdSpec(request.Name, request.ParentFolderId), cancellation);
                bool result = existingFolder != null && existingFolder.Id != request.Id;
                return !result;
            }).WithMessage(validationLocalizer["FolderNameAlreadyExistsInDirectory"])
            .When(x => x.Id is Guid && x.Id != Guid.Empty)
             .OverridePropertyName("Name");

        // Criação
        RuleFor(x => x)
            .MustAsync(async (request, cancellation) =>
            {
                bool result = await folderRepository.AnyAsync(new GetFolderByNameAndParentFolderIdSpec(request.Name, request.ParentFolderId), cancellation);
                return !result;
            })
            .WithMessage(validationLocalizer["FolderNameAlreadyExistsInDirectory"])
            .When(x => x.Id is null || x.Id == Guid.Empty)
             .OverridePropertyName("Name");
    }
}