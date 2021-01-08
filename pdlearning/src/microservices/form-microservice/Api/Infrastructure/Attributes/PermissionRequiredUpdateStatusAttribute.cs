using System.Linq;
using System.Text.Json;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Form.Infrastructure.Attributes
{
    public sealed class PermissionRequiredUpdateStatusAttribute : ActionFilterAttribute
    {
        public PermissionRequiredUpdateStatusAttribute()
        {
        }

        public void ValidatePermission(ActionExecutingContext context)
        {
            var paramInfo = context.ActionDescriptor.Parameters.FirstOrDefault(p => p.ParameterType == typeof(UpdateFormRequestDto));
            if (context.ActionArguments.TryGetValue(paramInfo?.Name, out object dto) && dto is UpdateFormRequestDto)
            {
                var request = dto as UpdateFormRequestDto;
                var userContext = context.HttpContext.RequestServices.GetService(typeof(IUserContext)) as IUserContext;
                var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<PermissionRequiredAttribute>)) as ILogger<PermissionRequiredAttribute>;
                string permission = string.Empty;

                switch (request.Status)
                {
                    case FormStatus.Published:
                        permission = CourseContentPermissionKeys.FormPublish;
                        break;
                    case FormStatus.Unpublished:
                        permission = CourseContentPermissionKeys.FormUnPublish;
                        break;
                    case FormStatus.PendingApproval:
                    case FormStatus.Approved:
                    case FormStatus.Rejected:
                    case FormStatus.ReadyToUse:
                    case FormStatus.Draft:
                    case FormStatus.Archived:
                        permission = CourseContentPermissionKeys.FormEdit;
                        break;
                }

                if (userContext.HasPermission(permission))
                {
                    return;
                }

                logger.LogError($"[PermissionDenied]: {JsonSerializer.Serialize(request)}");
                throw new BusinessLogicException("Access Denied");
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ValidatePermission(context);
        }
    }
}
