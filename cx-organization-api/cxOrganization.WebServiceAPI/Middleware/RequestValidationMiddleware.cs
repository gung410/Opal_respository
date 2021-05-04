using cxOrganization.Domain.AdvancedWorkContext;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using cxOrganization.Domain.Extensions;
using System.Text;

namespace cxOrganization.WebServiceAPI.Middleware
{
    public class RequestValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context, IAdvancedWorkContext workContext)
        {
            context.Request.EnableBuffering();
            /*
             Ref: https://devblogs.microsoft.com/aspnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
            This is the most recommendation from MS
             */

            // Leave the body open so the next middlewares can read it.
            using (var reader = new StreamReader(
               context.Request.Body,
               encoding: Encoding.UTF8,
               detectEncodingFromByteOrderMarks: false,
               leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                // Do some processing with body…
                if (body.IsValidJson() || string.IsNullOrEmpty(body))
                {
                    // Reset the request body stream position so the next middleware can read it
                    context.Request.Body.Position = 0;

                    await _next(context);
                }
                else
                {
                    await HandleExceptionBaseAsync(context);
                }

                // Because leaveOpen flag is true, the using scope's dispose() will be triggered automatically but not invoke stream.dispose().
                // So I need to invoke it manually here.
                reader.Close();
            }

        }

        private Task HandleExceptionBaseAsync(HttpContext context)
        {

            var code = HttpStatusCode.BadRequest;
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                error = "Payload is not valid json",
                errorCode = "400"
            },
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(message);
        }

    }
}
