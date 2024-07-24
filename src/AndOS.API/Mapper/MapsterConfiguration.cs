using AndOS.Application.Interfaces;
using AndOS.Domain.Entities;
using AndOS.Domain.Interfaces;
using AndOS.Infrastructure.Identity;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Update;
using AndOS.Shared.Requests.Files.Create;
using AndOS.Shared.Requests.Folders.Create;
using Mapster;

namespace AndOS.API.Mapper;

public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAccountRequest, Account>();
        config.NewConfig<UpdateAccountRequest, Account>();
        config.NewConfig<AccountDTO, Account>();
        config.AllowImplicitDestinationInheritance = true;
        config.AllowImplicitSourceInheritance = true;

        config.NewConfig<CreateFolderRequest, Folder>();
        config.NewConfig<Folder, FolderDTO>()
            .Inherits<SystemItem, SystemItemDTO>()
            .MaxDepth(3)
            .Map(dest => dest.Files, src => src.Files)
            .Map(dest => dest.Folders, src => src.Folders)
            .Map(dest => dest.ParentFolder, src => src.ParentFolder)
            .AfterMappingAsync(async (folder, folderDTO) =>
            {
                var systemItemService = MapContext.Current.GetService<ISystemItemService>();
                var currentUserContext = MapContext.Current.GetService<ICurrentUserContext>();
                var authorizationService = MapContext.Current.GetService<IAuthorizationService>();
                folderDTO.CurrentPath = await systemItemService.GetCurrentPath(folder);

                if (folderDTO.ParentFolder is null)
                    folderDTO.FullPath = folderDTO.Name;
                else
                    folderDTO.FullPath = $"{folderDTO.CurrentPath}/{folderDTO.Name}";

                var currentUserId = currentUserContext.GetCurrentUserId();
                folderDTO.Permissions = await authorizationService.GetFolderPermissionsFromUserAsync(folderDTO.Id, currentUserId);
                if (folder.Type != Core.Enums.FolderType.User)
                    folderDTO.CloudStorage = folder.GetAccount().CloudStorage;
            });

        config.NewConfig<Folder, ChildrenFolderDTO>()
            .AfterMapping((folder, folderDTO) =>
            {
                var currentUserContext = MapContext.Current.GetService<ICurrentUserContext>();
                var authorizationService = MapContext.Current.GetService<IAuthorizationService>();

                var currentUserId = currentUserContext.GetCurrentUserId();
                folderDTO.Permissions = authorizationService.GetFolderPermissionsFromUserAsync(folderDTO.Id, currentUserId).Result;
            });

        config.NewConfig<File, ChildrenFileDTO>()
            .AfterMapping((file, fileDTO) =>
            {
                var currentUserContext = MapContext.Current.GetService<ICurrentUserContext>();
                var authorizationService = MapContext.Current.GetService<IAuthorizationService>();

                var currentUserId = currentUserContext.GetCurrentUserId();
                fileDTO.Permissions = authorizationService.GetFilePermissionsFromUserAsync(fileDTO.Id, currentUserId).Result;
            });

        config.NewConfig<CreateFileRequest, File>();

        config.NewConfig<File, FileDTO>()
            .Inherits<SystemItem, SystemItemDTO>()
            .MaxDepth(2)
            .Map(dest => dest.ParentFolder, src => src.ParentFolder)
            .AfterMappingAsync(async (file, fileDTO) =>
            {
                var systemItemService = MapContext.Current.GetService<ISystemItemService>();
                var currentUserContext = MapContext.Current.GetService<ICurrentUserContext>();
                var authorizationService = MapContext.Current.GetService<IAuthorizationService>();

                fileDTO.CurrentPath = await systemItemService.GetCurrentPath(file);
                fileDTO.FullPath = $"{fileDTO.CurrentPath}/{fileDTO.Name}";

                Guid currentUserId = currentUserContext.GetCurrentUserId();
                fileDTO.Permissions = await authorizationService.GetFilePermissionsFromUserAsync(fileDTO.Id, currentUserId);
            });

    }
}