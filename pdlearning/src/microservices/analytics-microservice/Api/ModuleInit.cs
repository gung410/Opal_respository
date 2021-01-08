using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using MediatR;
using Microservice.Analytics.Application;
using Microservice.Analytics.Application.Services;
using Microservice.Analytics.Application.Services.Abstractions;
using Microservice.Analytics.Domain.ValueObject;
using Microservice.Analytics.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Analytics
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, AnalyticsSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, AnalyticsDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<AnalyticsDbContext>(ServiceLifetime.Transient);

            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(AnalyticsGenericRepository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(AnalyticsGenericRepositoryWithTypePrimaryKey<,>));

            RegisterCSLCommentServices(services);
            RegisterCSLLikeServices(services);
            RegisterCSLFileServices(services);
            RegisterLearnerBookmarkItemServices(services);
            RegisterPdpmPdPlanChangeStatusServices(services);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            services.AddSingleton<HttpService>();
        }

        protected void RegisterLearnerBookmarkItemServices(IServiceCollection services)
        {
            services.AddTransient<Func<AnalyticLearnerBookmarkItemType, IAnalyticsLearnerService>>(sp => key =>
            {
                switch (key)
                {
                    case AnalyticLearnerBookmarkItemType.Course:
                    case AnalyticLearnerBookmarkItemType.Microlearning:
                        return sp.GetService<AnalyticLearnerBookmarkCourseService>();
                    case AnalyticLearnerBookmarkItemType.DigitalContent:
                        return sp.GetService<AnalyticLearnerBookmarkDigitalContentService>();
                    case AnalyticLearnerBookmarkItemType.Community:
                        return sp.GetService<AnalyticLearnerBookmarkSpaceService>();
                    case AnalyticLearnerBookmarkItemType.LearningPath:
                        return sp.GetService<AnalyticLearnerLearnerLearningPathSpaceService>();
                    case AnalyticLearnerBookmarkItemType.LearningPathLMM:
                        return sp.GetService<AnalyticLearnerLMMLearningPathSpaceService>();
                    default:
                        throw new GeneralException($"AnalyticLearnerBookmarkItemType {key.ToString()} is not supported");
                }
            });
        }

        protected void RegisterCSLFileServices(IServiceCollection services)
        {
            services.AddTransient<Func<AnalyticCSLFileObjectModel, IAnalyticsCSLService>>(sp => key =>
            {
                switch (key)
                {
                    case AnalyticCSLFileObjectModel.Comment:
                        return sp.GetService<AnalyticsCSLFileCommentService>();
                    case AnalyticCSLFileObjectModel.ForumThread:
                        return sp.GetService<AnalyticsCSLFileForumThreadService>();
                    case AnalyticCSLFileObjectModel.Poll:
                        return sp.GetService<AnalyticsCSLFilePollService>();
                    case AnalyticCSLFileObjectModel.Post:
                        return sp.GetService<AnalyticsCSLFilePostService>();
                    case AnalyticCSLFileObjectModel.WikiPage:
                        return sp.GetService<AnalyticsCSLFileWikiPageService>();
                    default:
                        throw new GeneralException($"AnalyticCSLFileObjectModel {key.ToString()} is not supported");
                }
            });
        }

        protected void RegisterCSLLikeServices(IServiceCollection services)
        {
            services.AddTransient<Func<AnalyticCSLLikeSourceType, IAnalyticsCSLService>>(sp => key =>
            {
                switch (key)
                {
                    case AnalyticCSLLikeSourceType.Comment:
                        return sp.GetService<AnalyticsCSLLikeCommentService>();
                    case AnalyticCSLLikeSourceType.Forum:
                        return sp.GetService<AnalyticsCSLLikeForumService>();
                    case AnalyticCSLLikeSourceType.Poll:
                        return sp.GetService<AnalyticsCSLLikePollService>();
                    case AnalyticCSLLikeSourceType.Post:
                        return sp.GetService<AnalyticsCSLLikePostService>();
                    case AnalyticCSLLikeSourceType.Wiki:
                        return sp.GetService<AnalyticsCSLLikeWikiService>();
                    default:
                        throw new GeneralException($"AnalyticCSLLikeSourceType {key.ToString()} is not supported");
                }
            });
        }

        protected void RegisterCSLCommentServices(IServiceCollection services)
        {
            services.AddTransient<Func<AnalyticCSLCommentThreadType, IAnalyticsCSLService>>(sp => key =>
            {
                switch (key)
                {
                    case AnalyticCSLCommentThreadType.Forum:
                        return sp.GetService<AnalyticsCSLCommentForumThreadService>();
                    case AnalyticCSLCommentThreadType.Post:
                        return sp.GetService<AnalyticsCSLCommentPostService>();
                    case AnalyticCSLCommentThreadType.Poll:
                        return sp.GetService<AnalyticsCSLCommentPollService>();
                    case AnalyticCSLCommentThreadType.Wiki:
                        return sp.GetService<AnalyticsCSLCommentWikiService>();
                    default:
                        throw new GeneralException($"AnalyticCSLCommentThreadType {key.ToString()} is not supported");
                }
            });
        }

        protected void RegisterPdpmPdPlanChangeStatusServices(IServiceCollection services)
        {
            services.AddTransient<Func<AnalyticPdPlanActivity, IAnalyticsPdpmPdPlanStatusChangeService>>(sp => key =>
            {
                switch (key)
                {
                    case AnalyticPdPlanActivity.ActionItem:
                        return sp.GetService<AnalyticPdpmPdPlanActionItemStatusChangeService>();
                    case AnalyticPdPlanActivity.LearningDirection:
                        return sp.GetService<AnalyticPdpmPdPlanLearningDirectionStatusChangeService>();
                    case AnalyticPdPlanActivity.LearningNeed:
                        return sp.GetService<AnalyticPdpmPdPlanLearningNeedStatusChangeService>();
                    case AnalyticPdPlanActivity.LearningPlan:
                        return sp.GetService<AnalyticPdpmPdPlanLearningPlanStatusChangeService>();
                    case AnalyticPdPlanActivity.LearningProgramme:
                        return sp.GetService<AnalyticPdpmPdPlanLearningProgrammeStatusChangeService>();
                    default:
                        throw new GeneralException($"AnalyticsPdpmPdPlanStatusChange {key.ToString()} is not supported");
                }
            });
        }

        protected override List<IConventionalDependencyRegistrar> DeclareConventionalRegistrars()
        {
            var registrars = base.DeclareConventionalRegistrars();
            registrars.Add(new CqrsConventionalRegistrar());
            registrars.Add(new OpalRabbitMQConventionalRegistrar());
            registrars.Add(new SharedServiceConventionalRegistrar());

            return registrars;
        }
    }
}
