using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.StandaloneSurvey.Application;
using Microservice.StandaloneSurvey.Application.HangfireJob;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Services;
using Microservice.StandaloneSurvey.Domain.Versioning;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.StandaloneSurvey
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, StandaloneSurveySeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, StandaloneSurveyDbContextResolver>());

            // Inject Domain Services
            services.AddSingleton<ISurveyBusinessLogicService, SurveyBusinessLogicService>();

            // TODO: Remove application Services dependence bellow. implement service with ApplicationService instead.
            services.AddTransient<SurveyQuestionAnswerApplicationService>();
            services.AddTransient<SurveyAnswerApplicationService>();
            services.AddTransient<VersionTrackingApplicationService>();
            services.AddTransient<CommentApplicationService>();
            services.AddTransient<SurveySectionApplicationService>();
            services.AddTransient<SurveyParticipantApplicationService>();
            services.AddTransient<SurveyParticipantNotifyApplicationService>();
            services.AddTransient<WebAppLinkBuilder>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<StandaloneSurveyDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(StandaloneSurveyGenericRepository<>));

            services.AddTransient<ICheckoutVersionResolver, StandaloneSurveyVersionResolver>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone" } },
                { AuditLogActionType.Updated, new[] { "Mark", "Rollback", "Save", "Update" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AddingSubModuleInfoBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IAccessControlContext, AccessControlContext>();
            services.AddTransient<ICslAccessControlContext, CslAccessControlContext>();
            services.AddTransient<IArchiveFormScanner, ArchiveSurveyByArchiveDateScanner>();
            services.AddTransient<ICheckReferenceForArchiveFormScanner, ArchiveSurveyNotUsedForLongTimeScanner>();

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
