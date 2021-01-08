using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Microservice.Learner.Application
{
    public class SharedQueryConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<ISharedQuery>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
