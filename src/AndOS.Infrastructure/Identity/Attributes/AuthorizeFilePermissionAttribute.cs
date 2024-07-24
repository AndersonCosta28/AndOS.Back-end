using AndOS.Domain.Interfaces;
using AndOS.Resources.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace AndOS.Infrastructure.Identity.Attributes;

public enum FileFields
{
    Id
}

public class AuthorizeFilePermissionAttribute(FilePermission permission, FileFields field) : ActionFilterAttribute
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

            result = authorizationService.HasFilePermissionAsync(user, (Guid)valueProperty, permission).Result;

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
            FilePermission.Read => validationLocalizer["UnauthorizedRead"],
            FilePermission.Write => validationLocalizer["UnauthorizedWrite"],
            FilePermission.Rename => validationLocalizer["UnauthorizedRename"],
            FilePermission.Delete => validationLocalizer["UnauthorizedDelete"],
            FilePermission.Shared => validationLocalizer["UnauthorizedShare"],
            _ => null
        };

        return obj;
    }
}
