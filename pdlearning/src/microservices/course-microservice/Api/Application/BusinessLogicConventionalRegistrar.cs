using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Microservice.Course.Application
{
    public class BusinessLogicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<IBusinessLogic>())
                    .AsSelf()
                    .WithTransientLifetime());
        }
    }
}
