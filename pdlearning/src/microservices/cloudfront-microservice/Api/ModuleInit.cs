using System.Collections.Generic;
using Conexus.Opal.Microservice.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;

[assembly: ThunderModuleAssembly]

namespace Conexus.Opal.Microservice.CloudFront.Api
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddSingleton<HttpService>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());

            return registrars;
        }
    }
}
