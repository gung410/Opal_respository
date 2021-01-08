using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.NewsFeed.Infrastructure
{
    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<MongoOptions>()
                .Bind(configuration.GetSection(nameof(MongoOptions)));

            services.AddSingleton<NewsFeedDbClient>();
            services.AddScoped<NewsFeedDbContext>();

            services.AddTransient<IStartupFilter, MongoMigrationStartupFilter>();

            return services;
        }
    }
}
