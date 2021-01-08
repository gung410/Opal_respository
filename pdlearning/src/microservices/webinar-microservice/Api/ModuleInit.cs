using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using MediatR;
using Microservice.Webinar.Application;
using Microservice.Webinar.Application.Services;
using Microservice.Webinar.Application.Services.BigBlueButton;
using Microservice.Webinar.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Webinar
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IDbContextSeeder, WebinarSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, WebinarDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<WebinarDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(WebinarGenericRepository<>));

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone", "Create" } },
                { AuditLogActionType.Updated, new[] { "Change", "Update", "Save", "Cancel" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IUrlMeetingBuilder, UrlMeetingBuilder>();
            services.AddTransient<IWebinarUserService, WebinarUserService>();
            services.AddTransient<IRecordApplicationService, RecordApplicationService>();

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
