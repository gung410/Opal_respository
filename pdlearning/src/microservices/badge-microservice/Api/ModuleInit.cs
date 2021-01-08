using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.Badge.Application;
using Microservice.Badge.Application.BusinessLogic;
using Microservice.Badge.Application.HangfireJob;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;

[assembly: ThunderModuleAssembly]

namespace Microservice.Badge
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Create" } },
                { AuditLogActionType.Updated, new[] { "Change", "Update", "Save" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient<IEnsureYearlyStatisticExistLogic, EnsureYearlyStatisticExistLogic>();
            services.AddTransient(typeof(IProcessBadgeLogic), typeof(ProcessBadgeLogic));
            services.AddTransient<WebAppLinkBuilder>();

            services.AddTransient<ISummarizeDailyStatisticsForBadging, SummarizeDailyStatisticsJob>();
            services.AddTransient<ISummarizeMonthlyStatisticsForBadging, SummarizeMonthlyStatisticsJob>();
            services.AddSingleton<HttpService>();

            services.AddTransient(typeof(IGetBadgeCriteriaLogic<>), typeof(GetBadgeCriteriaLogic<>));

            // TODO: generic DI to register all BadgeCriteriaResolver automatically
            services.AddTransient<IGeneralBadgeCriteriaResolverLogic<CollaborativeLearnersBadgeCriteria, YearlyUserStatistic>, CollaborativeLearnersBadgeCriteriaResolverLogic>();
            services.AddTransient<IGeneralBadgeCriteriaResolverLogic<ReflectiveLearnersBadgeCriteria, YearlyUserStatistic>, ReflectiveLearnersBadgeCriteriaResolverLogic>();
            services.AddTransient<IGeneralBadgeCriteriaResolverLogic<DigitalLearnersBadgeCriteria, YearlyUserStatistic>, DigitalLearnersBadgeCriteriaResolverLogic>();
            services.AddTransient<IGeneralBadgeCriteriaResolverLogic<LifeLongBadgeCriteria, Guid>, LifeLongBadgeCriteriaResolverLogic>();
            services.AddTransient<IGeneralBadgeCriteriaResolverLogic<ActiveContributorsBadgeCriteria, Guid>, ActiveContributorsBadgeCriteriaResolverLogic>();

            // Community statistics.
            services.AddTransient<ICommunityBuilderBadgeCriteriaResolverLogic<LinkCuratorBadgeCriteria>, LinkCuratorBadgeCriteriaResolverLogic>();
            services.AddTransient<ICommunityBuilderBadgeCriteriaResolverLogic<VisualStorytellerBadgeCriteria>, VisualStorytellerBadgeCriteriaResolverLogic>();
            services
                .AddTransient<ICommunityBuilderBadgeCriteriaResolverLogic<ConversationStarterBadgeCriteria>,
                    ConversationStarterBadgeCriteriaResolverLogic>();
            services
                .AddTransient<ICommunityBuilderBadgeCriteriaResolverLogic<ConversationBoosterBadgeCriteria>,
                    ConversationBoosterBadgeCriteriaResolverLogic>();

            services.AddTransient<ICommunityStatisticLogic, CommunityStatisticLogic>();
            services.AddTransient<IProcessTopBadgeQualifiedUserLogic, ProcessTopBadgeQualifiedUserLogic>();
            services.AddTransient<WebAppLinkBuilder>();
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
