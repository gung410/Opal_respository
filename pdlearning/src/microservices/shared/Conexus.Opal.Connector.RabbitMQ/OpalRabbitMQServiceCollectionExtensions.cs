using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Conexus.Opal.Connector.RabbitMQ
{
    public static class OpalRabbitMQServiceCollectionExtensions
    {
        public static IServiceCollection AddOpalRabbitMQConnector(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddOptions<RabbitMQOptions>()
                .Bind(config.GetSection(nameof(RabbitMQOptions)));

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
            services.AddSingleton<IOpalMessageProducer, OpalMessageProducer>();
            services.AddHostedService<RabbitMQHostedService>();

            return services;
        }
    }
}
