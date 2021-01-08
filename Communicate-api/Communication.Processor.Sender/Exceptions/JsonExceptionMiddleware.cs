using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Communication.Processor.Sender.Exceptions
{
    public class JsonExceptionMiddleware
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public JsonExceptionMiddleware(IWebHostEnvironment hostingEnvironment, ILoggerFactory logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger.CreateLogger("JsonExceptionMiddleware");
        }
        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            HttpErrorDetails httpErrorDetail;

            if (ex is ApplicationBaseException)
            {
                var error = (ex as ApplicationBaseException);
                context.Response.StatusCode = (int)error.StatusCode;
                httpErrorDetail = new HttpErrorDetails()
                {
                    StatusCode = error.StatusCode,
                    Message = error.Message,
                    ErrorSource = error.Source,
                };
            }
            else
            {
                httpErrorDetail = new HttpErrorDetails()
                {
                    StatusCode = (HttpStatusCode)context.Response.StatusCode,
                    Message = ex.Message,
                    ErrorSource = ex.Source
                };
            }

            if (_hostingEnvironment.IsDevelopment() || _hostingEnvironment.IsEnvironment("localdev"))
            {
                httpErrorDetail.Detail = ex.StackTrace;
            }
            else
            {
                httpErrorDetail.Detail = ex.Message;
            }
            context.Response.ContentType = "application/json";
            _logger.LogError(ex, "An error occurred: {Application}");
            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, httpErrorDetail);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }

}
