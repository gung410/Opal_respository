using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using MediatR;
using Microservice.Learner.Application;
using Microservice.Learner.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Learner
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, LearnerSeeder>();

            // services.AddTransient<IEntityTypeMapper, PayslipRecordMapper>();
            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, LearnerDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<LearnerDbContext>(ServiceLifetime.Transient);

            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(LearnerGenericRepository<>));
            services.AddTransient(typeof(IReadOnlyRepository<>), typeof(LearnerReadGenericRepository<>));
            services.AddTransient(typeof(IWriteOnlyRepository<>), typeof(LearnerWriteGenericRepository<>));

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Create", "Enroll", "ReEnroll" } },
                { AuditLogActionType.Updated, new[] { "Change", "Complete", "Update", "Change" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddSingleton<HttpService>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());
            registrars.Add(new OpalRabbitMQConventionalRegistrar());
            registrars.Add(new BusinessLogicConventionalRegistrar());
            registrars.Add(new SharedQueryConventionalRegistrar());

            return registrars;
        }
    }
}
