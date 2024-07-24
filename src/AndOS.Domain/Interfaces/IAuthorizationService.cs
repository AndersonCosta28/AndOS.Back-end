using AndOS.Core.Enums;
using AndOS.Domain.Entities;
using System.Security.Claims;

namespace AndOS.Domain.Interfaces;

public interface IAuthorizationService
{
    Task<List<FilePermission>> GetFilePermissionsFromUserAsync(Guid fileId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<FolderPermission>> GetFolderPermissionsFromUserAsync(Guid folderId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<IUserClaim>> GetFileUserClaims(Guid fileId);
    Task<List<IUserClaim>> GetFolderUserClaims(Guid folderId);

    Task<bool> HasFolderPermissionAsync(ClaimsPrincipal user, string folderPath, FolderPermission permission, CancellationToken cancellationToken = default);
    Task<bool> HasFolderPermissionAsync(ClaimsPrincipal user, Guid? folderId, FolderPermission permission, CancellationToken cancellationToken = default);
    Task<bool> HasFilePermissionAsync(ClaimsPrincipal user, Guid fileId, FilePermission permission, CancellationToken cancellationToken = default);

    Task UpdatePermissionAsync(IUser user, IUserClaim claim, CancellationToken cancellationToken = default);
    Task UpdatePermissionAsync(IRole user, IRoleClaim claim, CancellationToken cancellationToken = default);

    IUserClaim CreateUserClaim(Guid folderId, FolderPermission permission, string value);
    IUserClaim CreateUserClaim(Guid fileId, FilePermission permission, string value);
}