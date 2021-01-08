using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Conexus.Opal.Connector.RabbitMQ.Dependency
{
    public class OpalRabbitMQConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<IOpalMessageConsumer>())
                    .AsImplementedInterfaces()
                    .AsSelf()
                    .WithTransientLifetime());
        }
    }
}
