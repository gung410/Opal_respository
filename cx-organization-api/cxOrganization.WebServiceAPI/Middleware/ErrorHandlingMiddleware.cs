using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain;
using cxOrganization.Domain.HttpClients;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using cxPlatform.Core.Extentions.Request;
using cxPlatform.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cxOrganization.WebServiceAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
            IWebHostEnvironment hostingEnvironment)
        {
            this.next = next;
            _hostingEnvironment = hostingEnvironment;
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context, IWorkContext workContext)
        {
            try
            {
                var cxRequestContext = context.Request.GetCxRequestContext(workContext);
                using (_logger.SetContextToScope(cxRequestContext))
                {
                    await next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (ex is IExceptionBase exceptionBase)
                {
                    await HandleExceptionBaseAsync(context, exceptionBase);
                }
                else
                {
                    await HandleExceptionAsync(context, ex);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = (ex is cxHttpResponseException cxException)
                ? cxException.StatusCode
                : HttpStatusCode.InternalServerError;
            context.Response.StatusCode = (int)statusCode;
            var errorMessage = GetErrorResponseMessage(statusCode, ex.Message);

            var httpErrorDetail = new HttpErrorDetails
            {
                StatusCode = statusCode,
                Message = errorMessage
            };

            if (ShowDetailError())
            {
                httpErrorDetail.ErrorSource = ex.Source;
                httpErrorDetail.Detail = ex.StackTrace;
            }

            context.Response.ContentType = "application/json";

            var jsonResponse = JsonConvert.SerializeObject(httpErrorDetail);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonResponse);
            MemoryStream stream = new MemoryStream(byteArray);
            await stream.CopyToAsync(context.Response.Body);
            await stream.DisposeAsync();

        }


        private Task HandleExceptionBaseAsync(HttpContext context, IExceptionBase currentException)
        {

            var code = currentException.GetErrorHttpStatusCode();
            var message = JsonConvert.SerializeObject(new
            {
                error = GetErrorResponseMessage(code, currentException.Message),
                errorCode = currentException.ErrorCode
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

        private string GetErrorResponseMessage(HttpStatusCode statusCode, string exceptionMessage)
        {

            if (ShowDetailError())
                return exceptionMessage;
            return (int)statusCode >= 500 ? "An expected error occurs on server." : exceptionMessage;

        }

        private bool ShowDetailError()
        {
            return _hostingEnvironment.IsDevelopment();
        }
    }
    public static class LogExtension
    {
        public static IRequestContext GetCxRequestContext(this HttpRequest request, IWorkContext workContext)
        {
            return new RequestContext(workContext)
            {
                RequestId = request.GetRequestId(),
                CorrelationId = request.GetCustomCorrelationId()
            };
        }
    }
    public static class cxStudioExceptionExtension
    {
        public static HttpStatusCode GetErrorHttpStatusCode(this IExceptionBase exception)
        {
            switch (exception.Exceptiontype)
            {
                case (cxStudioExceptionType.BadRequest):
                    return HttpStatusCode.BadRequest;
                case (cxStudioExceptionType.Conflict):
                    return HttpStatusCode.Conflict;
                case (cxStudioExceptionType.NotFound):
                    return HttpStatusCode.NotFound;
                case (cxStudioExceptionType.NoContent):
                    return HttpStatusCode.NoContent;
                case (cxStudioExceptionType.Forbidden):
                    return HttpStatusCode.Forbidden;
                case (cxStudioExceptionType.Unauthorized):
                    return HttpStatusCode.Unauthorized;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}