using Thunder.Platform.AspNetCore.Security;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityHeadersMiddlewareExtensions
    {
        /// <summary>
        /// Apply thunder unit of work middleware. This method should be called before UseMvc.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public static IApplicationBuilder UserThunderSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
