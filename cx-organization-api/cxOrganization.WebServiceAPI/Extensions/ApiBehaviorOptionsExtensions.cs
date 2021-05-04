using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace cxOrganization.WebServiceAPI.Extensions
{
    public static class ApiBehaviorOptionsExtensions
    {
        public static void ConfigureModelBindingExceptionHandling(IServiceCollection services, ILogger logger)
        {

            services.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
            {
                apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
                {
                    ValidationProblemDetails error = actionContext.ModelState
                        .Where(e => e.Value.Errors.Any())
                        .Select(e => new ValidationProblemDetails(actionContext.ModelState))
                        .FirstOrDefault();

                    // Here you can add logging for troubleshooting.
                    var referrer = actionContext.HttpContext.Request.Headers["Referer"].ToString();
                    logger.LogError($"{actionContext.HttpContext.Request.Path.Value} received invalid message format: {error.Errors.Values}. Referrer {referrer}");

                    // Return new object to not expose the detail of the bad request. Digging into the log to see more details.
                    return new BadRequestObjectResult(new ValidationProblemDetails());

                };

            });
        }
    }
}
