using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ThunderUnitOfWorkMiddlewareExtensions
    {
        /// <summary>
        /// Apply thunder unit of work middleware. This method should be called before UseMvc.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        /// <param name="optionsAction">The options configuration action.</param>
        /// <returns>The IApplicationBuilder.</returns>
        public static IApplicationBuilder UseThunderUnitOfWork([NotNull] this IApplicationBuilder app, Action<UnitOfWorkMiddlewareOptions> optionsAction = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = app.ApplicationServices.GetRequiredService<IOptions<UnitOfWorkMiddlewareOptions>>().Value;
            optionsAction?.Invoke(options);
            return app.UseMiddleware<ThunderUnitOfWorkMiddleware>();
        }
    }
}
