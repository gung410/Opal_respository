using System;
using Conexus.Opal.OutboxPattern.HostedServices;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.HostedServices;
using Conexus.Opal.OutboxPattern.Variants.MongoDb.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conexus.Opal.OutboxPattern
{
    public static class OutboxPatternServiceCollectionExtensions
    {
        public static IServiceCollection AddOutboxQueueMessage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection(nameof(OutboxOptions)));

            services.AddTransient<IOutboxQueue, OutboxQueue>();
            services.AddHostedService<OutboxMessageSender>();
            services.AddHostedService<OutboxMessageCleaner>();

            return services;
        }

        public static IServiceCollection AddMongoOutboxQueueMessage<T>(this IServiceCollection services, IConfiguration configuration) where T : IHasOutboxCollection
        {
            services.AddOptions<OutboxOptions>().Bind(configuration.GetSection(nameof(OutboxOptions)));

            services.AddScoped<IHasOutboxCollection>(p => p.GetService<T>());

            services.AddTransient<IOutboxQueue, MongoOutboxQueue>();
            services.AddHostedService<MongoOutboxMessageSender>();
            services.AddHostedService<MongoOutboxMessageCleaner>();

            return services;
        }
    }
}
