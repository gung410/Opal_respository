using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using MediatR;
using Microservice.Calendar.Application;
using Microservice.Calendar.Application.Services;
using Microservice.Calendar.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Calendar
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, CalendarSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, CalendarDbContextResolver>());

            services.AddTransient<PersonalCalendarApplicationService>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CalendarDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(CalendarGenericRepository<>));

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Create" } },
                { AuditLogActionType.Updated, new[] { "Save", "Update", "Change" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<ICalendarEventNotifierService, CalendarEventNotifierService>();

            // Configure route constraints.
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("CalendarEventSource", typeof(Application.RouteContraints.CalendarEventSourceRouteConstraint));
            });

            services.AddSingleton<HttpService>();
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());
            registrars.Add(new OpalRabbitMQConventionalRegistrar());
            registrars.Add(new SharedQueryConventionalRegistrar());

            return registrars;
        }
    }
}
