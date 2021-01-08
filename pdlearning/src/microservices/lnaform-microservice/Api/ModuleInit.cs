using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.LnaForm.Application;
using Microservice.LnaForm.Application.HangfireJob;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Services;
using Microservice.LnaForm.Domain.Versioning;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Application.Services;
using Microservice.LnaForm.Versioning.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.LnaForm
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, LnaFormSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, LnaFormDbContextResolver>());

            // Inject Domain Services
            services.AddSingleton<IFormBusinessLogicService, FormBusinessLogicService>();

            // TODO: Remove application Services dependence bellow. implement service with ApplicationService instead.
            services.AddTransient<FormQuestionAnswerApplicationService>();
            services.AddTransient<FormAnswerApplicationService>();
            services.AddTransient<VersionTrackingApplicationService>();
            services.AddTransient<CommentApplicationService>();
            services.AddTransient<FormSectionApplicationService>();
            services.AddTransient<FormParticipantApplicationService>();
            services.AddTransient<FormParticipantNotifyApplicationService>();
            services.AddTransient<WebAppLinkBuilder>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<LnaFormDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(LnaFormGenericRepository<>));

            services.AddTransient<ICheckoutVersionResolver, FormVersionResolver>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone" } },
                { AuditLogActionType.Updated, new[] { "Mark", "Rollback", "Save", "Update" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IAccessControlContext, AccessControlContext>();
            services.AddTransient<IArchiveFormScanner, ArchiveFormByArchiveDateScanner>();
            services.AddTransient<ICheckReferenceForArchiveFormScanner, ArchiveFormNotUsedForLongTimeScanner>();

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
