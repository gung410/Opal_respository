using System.IO;
using System.Text;

using Ganss.XSS;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace cxOrganization.WebServiceAPI.ActionFilters
{
    public class PreventXSSFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting (ResourceExecutingContext context)
        {
            var request = context.HttpContext.Request;
            if (request.ContentLength > 0)
            {
                request.EnableBuffering();
                request.Body.Position = 0;
                using (var reader = new StreamReader(request.Body))
                {
                    var originalString = reader.ReadToEndAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(originalString))
                    {
                        var sanitizer = new HtmlSanitizer();
                        var sanitized = sanitizer.Sanitize(originalString);
                        byte[] bytes = Encoding.UTF8.GetBytes(sanitized);
                        request.Body = new MemoryStream(bytes);
                    }
                }
            }
        }
    }
}
