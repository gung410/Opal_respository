using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Service.Authentication
{
    public sealed class PermissionRequiredAttribute : ActionFilterAttribute
    {
        public PermissionRequiredAttribute(params string[] permissionKeys)
        {
            PermissionKeys = permissionKeys;
        }

        public string[] PermissionKeys { get; }

        public void ValidatePermission(ActionExecutingContext context)
        {
            var userContext = context.HttpContext.RequestServices.GetService(typeof(IUserContext)) as IUserContext;
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<PermissionRequiredAttribute>)) as ILogger<PermissionRequiredAttribute>;
            if (userContext.HasPermission(PermissionKeys))
            {
                return;
            }

            logger.LogError($"[PermissionDenied]: {PermissionKeys}");
            throw new BusinessLogicException("Access Denied");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ValidatePermission(context);
        }
    }
}
