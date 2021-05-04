using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Services;
using cxOrganization.WebServiceAPI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxAPI.Competence.Base.Middleware
{
    /// <summary>
    /// Init authorization data for the request.
    /// </summary>
    public class InitAuthorizationDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public InitAuthorizationDataMiddleware(
              IServiceScopeFactory serviceScopeFactory,
              RequestDelegate next)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context, IAdvancedWorkContext workContext)
        {
            var (ownerId, customerId) = context.Request.GetCustomerAndOnwerIds();

            if (ShouldGetUserPermissionKeys(context, workContext))
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var _portalApiClient = serviceScope.ServiceProvider.GetService<IPortalApiClient>();
                    workContext.CurrentUserPermissionKeys = await _portalApiClient.GetPermissionKeys(workContext.OriginalTokenString, $"{ownerId}:{customerId}");
                }
            }

            await _next(context);
        }

        private bool ShouldGetUserPermissionKeys(HttpContext context, IAdvancedWorkContext workContext)
        {
            // TODO: Filter the Method and Endpoints which should be validated authorization.
            // i.e: For now, the system just needs to validate authorization on endpoints which modifying the data.

            var methods = new List<string>()
            {
                "GET",
                "POST",
                "PUT",
                "DELETE"
            };

            return methods.Contains(context.Request.Method)
                && context.Request.Path.HasValue 
                && context.Request.Path.Value != "/"
                && !string.IsNullOrEmpty(workContext.OriginalTokenString)
                && !workContext.isInternalRequest;
        }
    }
}
