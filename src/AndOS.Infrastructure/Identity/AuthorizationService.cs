using AndOS.Application.RoleClaim.Get;
using AndOS.Application.UserClaim.Get;
using AndOS.Application.UserGet;
using AndOS.Domain.Interfaces;
using AndOS.Infrastructure.Exceptions;
using AndOS.Infrastructure.Identity.Entities;
using AndOS.Resources.Localization;
using AndOS.Shared.Requests.Folders.Get.GetByPath;
using MediatR;
using Microsoft.Extensions.Localization;

namespace AndOS.Infrastructure.Identity;

public class AuthorizationService(ISender sender,
    IRepository<IUserClaim> userClaimRepository,
    IRepository<IRoleClaim> roleClaimRepository,
    ICurrentUserContext currentUserContext,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IStringLocalizer<ValidationResource> validationLocalizer) : IAuthorizationService
{
    public async Task<bool> HasFolderPermissionAsync(ClaimsPrincipal user, string folderPath, FolderPermission permission, CancellationToken cancellationToken = default)
    {
        var requestGetFolderByPath = new GetFolderByPathRequest(folderPath);
        var folder = await sender.Send(requestGetFolderByPath, cancellationToken);

        if (folder.ParentFolder == null && folder.Id == Guid.Empty && folder.Name == string.Empty)
            return true;

        return await HasFolderPermissionAsync(user, folder.Id, permission, cancellationToken);
    }

    public async Task<bool> HasFolderPermissionAsync(ClaimsPrincipal user, Guid? folderId, FolderPermission permission, CancellationToken cancellationToken = default)
    {
        var userId = currentUserContext.GetCurrentUserId();
        var claimType = CustomClaimTypeFolder(folderId, permission);
        var hasPermission = await userClaimRepository.AnyAsync(new GetUserClaimByUserIdAndTypeAndValueSpec(userId, claimType, ClaimConsts.VALUE_TRUE), cancellationToken);
        return hasPermission;
    }

    public async Task<bool> HasFilePermissionAsync(ClaimsPrincipal user, Guid fileId, FilePermission permission, CancellationToken cancellationToken = default)
    {
        var userId = currentUserContext.GetCurrentUserId();
        var claimType = CustomClaimTypeFile(fileId, permission);
        var hasPermission = await userClaimRepository.AnyAsync(new GetUserClaimByUserIdAndTypeAndValueSpec(userId, claimType, ClaimConsts.VALUE_TRUE), cancellationToken);

        return hasPermission;
    }

    public async Task<List<FilePermission>> GetFilePermissionsFromUserAsync(Guid fileId, Guid userId = default, CancellationToken cancellationToken = default)
    {
        if (userId == default)
            userId = currentUserContext.GetCurrentUserId();
        var claims = await userClaimRepository.ListAsync(new GetFileUserClaimByUserIdAndFileIdSpec(userId, fileId), cancellationToken);

        var permissions = claims.Select(claim => claim.ClaimType.Split(".")[1])
                                .Select(getFilePermissionByDescription)
                                .ToList();

        return permissions;
    }

    public async Task<List<FolderPermission>> GetFolderPermissionsFromUserAsync(Guid folderId, Guid userId = default, CancellationToken cancellationToken = default)
    {
        if (userId == default)
            userId = currentUserContext.GetCurrentUserId();

        var claims = await userClaimRepository.ListAsync(new GetFolderUserClaimByUserIdAndFolderIdSpec(userId, folderId), cancellationToken);

        var permissions = claims
                                             .Select(andOSClaim => andOSClaim.ClaimType.Split(".")[1])
                                             .Select(getFolderPermissionByDescription)
                                             .ToList();

        return permissions;
    }

    public async Task<List<IUserClaim>> GetFileUserClaims(Guid fileId)
    {
        var claims = await userClaimRepository.ListAsync(new GetFileUserClaimSpec(fileId, ClaimConsts.VALUE_TRUE));
        return claims;
    }

    public async Task<List<IUserClaim>> GetFolderUserClaims(Guid folderId)
    {
        var claims = await userClaimRepository.ListAsync(new GetFolderUserClaimSpec(folderId, ClaimConsts.VALUE_TRUE));
        return claims;
    }

    private FilePermission getFilePermissionByDescription(string description)
    {
        var valuesInEnum = Enum.GetValues<FilePermission>();
        return valuesInEnum.First(x => x.GetDescription() == description);
    }

    private FolderPermission getFolderPermissionByDescription(string description)
    {
        var valuesInEnum = Enum.GetValues<FolderPermission>();
        return valuesInEnum.First(x => x.GetDescription() == description);
    }

    public async Task UpdatePermissionAsync(IUser user, IUserClaim claim, CancellationToken cancellationToken = default)
    {
        var userClaim = await userClaimRepository.FirstOrDefaultAsync(new GetUserClaimByUserIdAndType(user.Id, claim.ClaimType), cancellationToken);

        if (userClaim != null)
        {
            if (userClaim.ClaimValue == claim.ClaimValue)
                return;

            userClaim.ClaimValue = claim.ClaimValue;
            await userClaimRepository.UpdateAsync(userClaim, cancellationToken);
        }
        else
            await userManager.AddClaimAsync((ApplicationUser)user, claim.ToClaim());
    }

    public async Task UpdatePermissionAsync(IRole role, IRoleClaim claim, CancellationToken cancellationToken = default)
    {
        var applicationRoleClaim = await roleClaimRepository.FirstOrDefaultAsync(new GetRoleClaimByRoleIdAndType(role.Id, claim.ClaimType), cancellationToken);

        if (applicationRoleClaim != null)
        {
            if (applicationRoleClaim.ClaimValue == claim.ClaimValue)
                return;

            applicationRoleClaim.ClaimValue = claim.ClaimValue;
            await roleClaimRepository.UpdateAsync(applicationRoleClaim, cancellationToken);
        }
        else
            await roleManager.AddClaimAsync((ApplicationRole)role, claim.ToClaim());
    }

    public IUserClaim CreateUserClaim(Guid folderId, FolderPermission permission, string value)
    {
        if (folderId == Guid.Empty)
            throw new InfrastructureLayerException(validationLocalizer["FolderIdCannotBeNullOrEmpty"]);
        var result = new ApplicationUserClaim
        {
            ClaimType = CustomClaimTypeFolder(folderId, permission),
            ClaimValue = value,
        };
        return result;
    }

    public IUserClaim CreateUserClaim(Guid fileId, FilePermission permission, string value)
    {
        if (fileId == Guid.Empty)
            throw new InfrastructureLayerException(validationLocalizer["FileIdCannotBeNullOrEmpty"]);
        var result = new ApplicationUserClaim
        {
            ClaimType = CustomClaimTypeFile(fileId, permission),
            ClaimValue = value,
        };
        return result;
    }

    public static string CustomClaimTypeFolder(Guid? folderId, FolderPermission permission) =>
        $"{ClaimConsts.SCOPE_FOLDER}.{Enum.GetName(typeof(FolderPermission), permission)}.{folderId}";

    public static string CustomClaimTypeFile(Guid fileId, FilePermission permission) =>
        $"{ClaimConsts.SCOPE_FILE}.{Enum.GetName(typeof(FilePermission), permission)}.{fileId}";
}