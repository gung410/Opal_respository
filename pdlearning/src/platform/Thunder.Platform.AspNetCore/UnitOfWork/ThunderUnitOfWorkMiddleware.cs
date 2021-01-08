using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Thunder.Platform.AspNetCore.UnitOfWork
{
    public class ThunderUnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public ThunderUnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            [NotNull] HttpContext httpContext,
            [NotNull] IUnitOfWorkManager unitOfWorkManager,
            [NotNull] IOptions<UnitOfWorkMiddlewareOptions> options)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (unitOfWorkManager == null)
            {
                throw new ArgumentNullException(nameof(unitOfWorkManager));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.Value.Filter(httpContext))
            {
                await _next(httpContext);
                return;
            }

            using (var uow = unitOfWorkManager.Begin(options.Value.OptionsFactory(httpContext)))
            {
                await _next(httpContext);
                await uow.CompleteAsync();
            }
        }
    }
}
