using System;
using System.Collections.Generic;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace cxOrganization.WebServiceAPI.Swagger
{
    public class cxTokenHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null) return;

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var routeInfo = context.ApiDescription.ActionDescriptor.AttributeRouteInfo;
            if (routeInfo != null)
            {
                if (!routeInfo.Template.StartsWith("owners/{ownerid}", StringComparison.OrdinalIgnoreCase))
                {
                    var parameter = new OpenApiParameter
                    {
                        Description = "OwnerId:CustomerId",
                        In = ParameterLocation.Header,
                        Name = "cxToken",
                        Required = true
                    };
                    operation.Parameters.Add(parameter);
                }
            }
        }
    }
}
