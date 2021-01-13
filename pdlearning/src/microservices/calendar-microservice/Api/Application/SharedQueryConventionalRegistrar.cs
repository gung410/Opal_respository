using Microservice.Calendar.Application.SharedQueries.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Microservice.Calendar.Application
{
    public class SharedQueryConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<ISharedQuery>())
                .AsSelf()
                .WithTransientLifetime());
        }
    }
}