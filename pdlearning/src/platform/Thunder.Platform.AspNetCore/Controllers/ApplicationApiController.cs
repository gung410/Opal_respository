using System;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Platform.AspNetCore.Controllers
{
    public abstract class ApplicationApiController : ControllerBase
    {
        protected ApplicationApiController(IUserContext userContext)
        {
            UserContext = userContext;
        }

        protected IUserContext UserContext { get; }

        protected Guid CurrentUserId
        {
            get
            {
                var userId = UserContext.GetValue<string>(CommonUserContextKeys.UserId);
                return !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : Guid.Empty;
            }
        }

        protected FileContentResult DownloadFile(byte[] fileContent, string contentType, string fullFileName)
        {
            Response.Headers.Add("Download-File-Name", fullFileName);
            Response.Headers.Add("Access-Control-Expose-Headers", "Download-File-Name");
            return File(fileContent, contentType, fullFileName);
        }

        protected void EnsureValidPermission(bool checkHasPermission)
        {
            if (!checkHasPermission)
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }
        }
    }
}
