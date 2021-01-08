using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.Content.Application;
using Microservice.Content.Application.HangfireJob;
using Microservice.Content.Domain.Versioning;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Content
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, CourseSeeder>();

            // services.AddTransient<IEntityTypeMapper, CourseRecordMapper>();
            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, ContentDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ContentDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(ContentGenericRepository<>));

            // Inject checkout resolver service collection
            services.AddTransient<ICheckoutVersionResolver, DigitalContentVersionResolver>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone", "Create" } },
                { AuditLogActionType.Updated, new[] { "Mark", "Change", "Rename", "Rollback", "Save" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IAccessControlContext, AccessControlContext>();
            services.AddTransient<INotifyContentByExpiredDateScanner, NotifyContentByExpiredDateScanner>();

            services.AddTransient<IDigitalContentDailyCheckingJob, DigitalContentDailyCheckingJob>();
            services.AddTransient<IDigitalContentWeeklyCheckingJob, DigitalContentWeeklyCheckingJob>();

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
