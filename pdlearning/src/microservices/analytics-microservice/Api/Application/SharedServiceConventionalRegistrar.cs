using Microservice.Analytics.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Microservice.Analytics.Application
{
    public class SharedServiceConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<IAnalyticsShareService>())
                .AsSelf()
                .WithTransientLifetime());
        }
    }
}
