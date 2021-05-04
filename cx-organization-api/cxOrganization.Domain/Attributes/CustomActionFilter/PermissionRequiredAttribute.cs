using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.DomainEnums;
using cxPlatform.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace cxOrganization.Domain.Attributes.CustomActionFilters
{
    public sealed class PermissionRequiredAttribute : ActionFilterAttribute
    {
        public string[] PermissionKeys { get; }
        public PermissionRequiredAttribute(params string[] permissionKeys)
        {
            this.PermissionKeys = permissionKeys;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ValidatePermission(context);
        }

        private void ValidatePermission(ActionExecutingContext context)
        {
            var workContext = context.HttpContext.RequestServices.GetService(typeof(IAdvancedWorkContext)) as IAdvancedWorkContext;
            if (!workContext.HasPermission(PermissionKeys) && !workContext.isInternalRequest)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_ACCESS_DENIED_USER, PermissionErrorMessage.No_PERMISSION);
            }
        }
    }
}
