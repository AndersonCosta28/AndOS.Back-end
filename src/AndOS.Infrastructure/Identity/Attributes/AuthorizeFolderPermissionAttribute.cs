using AndOS.Domain.Interfaces;
using AndOS.Resources.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace AndOS.Infrastructure.Identity.Attributes;
public enum FolderFields
{
    ParentFolderId,
    Id,
    Path
}

public class AuthorizeFolderPermissionAttribute(FolderPermission permission, FolderFields field) : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var request = context.ActionArguments.Values.FirstOrDefault(v => v.GetType().GetProperty(field.ToString()) != null);

        if (request != null)
        {
            var property = request.GetType().GetProperty(field.ToString());
            var valueProperty = property.GetValue(request);

            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            bool result;

            if (field is FolderFields.ParentFolderId or FolderFields.Id)
                result = authorizationService.HasFolderPermissionAsync(user, (Guid)valueProperty, permission).Result;
            else
                result = authorizationService.HasFolderPermissionAsync(user, (string)valueProperty, permission).Result;

            if (!result)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = GetMessage(context).Value,
                    ContentType = "text/plain",
                };
            }
        }

        base.OnActionExecuting(context);
    }

    LocalizedString GetMessage(ActionExecutingContext context)
    {
        var validationLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<ValidationResource>>();

        var obj = permission switch
        {
            FolderPermission.Read => validationLocalizer["UnauthorizedRead"],
            FolderPermission.Write => validationLocalizer["UnauthorizedWrite"],
            FolderPermission.Rename => validationLocalizer["UnauthorizedRename"],
            FolderPermission.Delete => validationLocalizer["UnauthorizedDelete"],
            FolderPermission.Shared => validationLocalizer["UnauthorizedShare"],
            _ => null
        };

        return obj;
    }
}
