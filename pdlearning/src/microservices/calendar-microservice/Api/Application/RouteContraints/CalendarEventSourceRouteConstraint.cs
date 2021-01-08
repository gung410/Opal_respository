using System;
using Microservice.Calendar.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microservice.Calendar.Application.RouteContraints
{
    public class CalendarEventSourceRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var type = values[routeKey]?.ToString();
            return Enum.TryParse<CalendarEventSource>(type, ignoreCase: true, out CalendarEventSource result);
        }
    }
}
