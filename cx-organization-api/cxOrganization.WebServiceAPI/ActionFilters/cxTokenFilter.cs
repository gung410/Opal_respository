using System;
using System.Collections.Generic;
using System.Linq;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Extensions;
using cxPlatform.Core;
using cxPlatform.Core.Extentions.Request;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace cxOrganization.WebServiceAPI.ActionFilters
{
    public class cxTokenFilter : IActionFilter
    {
        private static HashSet<string> _routesSkipCheckingCxToken = new HashSet<string>
        {
            "apiversions",
            "owners/{ownerId}/",
            "odata/"
        };
        private readonly IAdvancedWorkContext _workContext;
        private readonly AppSettings _appSettings;
        public cxTokenFilter(IAdvancedWorkContext workContext,
            IOptions<AppSettings> options)
        {
            _workContext = workContext;
            _appSettings = options.Value;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _workContext.CurrentUserId = _appSettings.CurrentUserId;
            var routeTemplate = context.ActionDescriptor.AttributeRouteInfo.Template;
            if (_routesSkipCheckingCxToken.Any(t=> routeTemplate.StartsWith(t)))
            {
                _workContext.IsEnableFiltercxToken = false;
                return;
            }
            var (ownerId, customerId) = context.HttpContext.Request.GetCustomerAndOnwerIds();
            _workContext.IsEnableFiltercxToken = true;
            _workContext.CurrentOwnerId = ownerId;
            _workContext.CurrentCustomerId = customerId;
            _workContext.RequestId = context.HttpContext.Request.GetRequestId();
            _workContext.CorrelationId = context.HttpContext.Request.GetCustomCorrelationId();
            _workContext.UserIdCXID = context.HttpContext.GetUserIdFromCXID();
        }
    }
}
