using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Platform.AspNetCore.Validation
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ThunderModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelState = context.ModelState;
            if (!modelState.IsValid)
            {
                throw new DataValidationException(modelState.BuildErrorResponse());
            }
        }
    }
}
