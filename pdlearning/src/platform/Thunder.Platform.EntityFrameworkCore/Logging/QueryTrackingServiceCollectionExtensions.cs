using Microsoft.Extensions.DependencyInjection;

namespace Thunder.Platform.EntityFrameworkCore.Logging
{
    public static class QueryTrackingServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryTrackingSupport(this IServiceCollection services)
        {
            services.AddScoped<IQueryTrackingSource, ThunderQueryTrackingSource>();

            return services;
        }
    }
}
