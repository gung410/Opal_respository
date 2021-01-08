using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Middleware
{
    public class AddXSSProtectionHeaderMiddleware
    {
        private readonly RequestDelegate next;

        public AddXSSProtectionHeaderMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            await next(context);
        }
    }
}
