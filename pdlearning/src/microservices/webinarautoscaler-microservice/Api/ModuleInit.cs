using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using Microservice.WebinarAutoscaler.Application.HostedServices;
using Microservice.WebinarAutoscaler.Application.Services;
using Microservice.WebinarAutoscaler.Application.Services.AWSServices;
using Microservice.WebinarAutoscaler.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.WebinarAutoscaler
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IDbContextSeeder, WebinarAutoscalerSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, WebinarAutoscalerContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<WebinarAutoscalerDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(WebinarAutoscalerGenericRepository<>));

            services.AddTransient<IEC2InstanceService, EC2InstanceService>();
            services.AddTransient<IALBService, ALBService>();
            services.AddTransient<IBBBSyncService, BBBSyncService>();
            services.AddTransient<IBBBServerService, BBBServerService>();

            services.AddHostedService<ScaleOutBBBServerHostedService>();
            services.AddHostedService<ScaleInBBBServerHostedService>();
            services.AddHostedService<SeekCoordinatedBBBServerService>();

            services.AddSingleton<HttpService>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());
            registrars.Add(new OpalRabbitMQConventionalRegistrar());

            return registrars;
        }
    }
}
