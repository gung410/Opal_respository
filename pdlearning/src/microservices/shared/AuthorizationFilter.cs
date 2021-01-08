using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Thunder.Platform.Core.Context;

namespace Thunder.Service.Authentication
{
    public class AuthorizationFilter : IActionFilter
    {
        private readonly IUserContext _userContext;

        public AuthorizationFilter(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasUserId = !string.IsNullOrEmpty(_userContext.GetValue<string>(CommonUserContextKeys.UserId));
            var validThunderSecretKey = _userContext.GetValue<bool>(CommonUserContextKeys.ValidThunderSecretKey);

            if (hasUserId || validThunderSecretKey)
            {
                return;
            }

            throw new AuthenticationException();
        }
    }
}
