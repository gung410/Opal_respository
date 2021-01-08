using System.Collections.Generic;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.Form.Application;
using Microservice.Form.Application.HangfireJob;
using Microservice.Form.Application.Services;
using Microservice.Form.Application.SharedQueries;
using Microservice.Form.Domain;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.Versioning;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Form
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, FormSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, FormDbContextResolver>());

            // Inject Domain Services
            services.AddSingleton<IFormAnswerScoreCalculationService, FormAnswerScoreCalculationService>();
            services.AddSingleton<IFormBusinessLogicService, FormBusinessLogicService>();

            // TODO: Remove application Services dependence bellow. implement service with ApplicationService instead.
            services.AddTransient<FormQuestionAnswerApplicationService>();
            services.AddTransient<FormParticipantApplicationService>();
            services.AddTransient<VersionTrackingApplicationService>();
            services.AddTransient<FormSectionApplicationService>();
            services.AddTransient<FormAnswerApplicationService>();
            services.AddTransient<FormNotifyApplicationService>();
            services.AddTransient<CommentApplicationService>();
            services.AddTransient<GetFormsSharedQuery>();
            services.AddTransient<WebAppLinkBuilder>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<FormDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();
            services.AddTransient(typeof(IRepository<>), typeof(FormGenericRepository<>));
            services.AddTransient<ICheckoutVersionResolver, FormVersionResolver>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone" } },
                { AuditLogActionType.Updated, new[] { "Mark", "Rollback", "Save", "Update" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddTransient<IAccessControlContext, FormAccessControlContext>();

            services.AddTransient<IFormDailyCheckingJob, FormDailyCheckingJob>();
            services.AddTransient<IFormWeeklyCheckingJob, FormWeeklyCheckingJob>();

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
