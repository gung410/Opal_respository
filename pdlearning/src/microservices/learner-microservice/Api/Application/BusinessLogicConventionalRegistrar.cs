using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;

namespace Microservice.Learner.Application
{
    public class BusinessLogicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.Services.Scan(scan => scan
                .FromAssemblies(context.Assembly)

                // Register transient instances.
                .AddClasses(@class => @class.AssignableTo<IBusinessLogic>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }
    }
}
