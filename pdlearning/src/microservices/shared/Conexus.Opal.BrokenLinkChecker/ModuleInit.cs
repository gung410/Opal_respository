using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;

[assembly: ThunderModuleAssembly]

namespace Conexus.Opal.BrokenLinkChecker
{
    [DependsOn(typeof(ThunderKernelModule))]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IBrokenLinkChecker, BrokenLinkChecker>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();

            return registrars;
        }
    }
}
