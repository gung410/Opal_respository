using System.Collections.Generic;
using System.Collections.Immutable;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Dependency;
using Conexus.Opal.Microservice.Infrastructure;
using Conexus.Opal.Microservice.Metadata.DataProviders;
using Conexus.Opal.Microservice.Metadata.DataSync;
using cx.datahub.scheduling.jobs.shared;
using MediatR;
using Microservice.Course.Application;
using Microservice.Course.Application.HangfireJob.RecurringJob;
using Microservice.Course.Application.Services;
using Microservice.Course.Application.Validators.FilterCondition;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Infrastructure;
using Microservice.Course.Metadata.DataProviders;
using Microservice.Course.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thunder.Platform.Core.Dependency;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Modules;
using Thunder.Platform.Cqrs.Dependency;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Logging;

[assembly: ThunderModuleAssembly]

namespace Microservice.Course
{
    [StartupModule]
    public class ModuleInit : ThunderModule
    {
        protected override void InternalRegisterTo(IServiceCollection services)
        {
            services.AddTransient<IDbContextSeeder, CourseSeeder>();

            services.Replace(ServiceDescriptor.Scoped<IDbContextResolver, CourseDbContextResolver>());

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CourseDbContext>(ServiceLifetime.Transient);
            services.AddQueryTrackingSupport();

            services.AddTransient(typeof(IRepository<>), typeof(CourseGenericRepository<>));
            services.AddTransient(typeof(IReadOnlyRepository<>), typeof(CourseReadGenericRepository<>));
            services.AddTransient(typeof(IWriteOnlyRepository<>), typeof(CourseWriteGenericRepository<>));

            // Inject Application Services
            services.AddTransient<AssignmentService>();
            services.AddTransient<CourseService>();
            services.AddTransient<ClassRunService>();
            services.AddTransient<RegistrationService>();
            services.AddTransient<SessionService>();
            services.AddTransient<AttendanceTrackingService>();
            services.AddTransient<LearningPathService>();
            services.AddTransient<CommentService>();
            services.AddTransient<CoursePlanningCycleService>();
            services.AddTransient<ECertificateService>();
            services.AddTransient<LearningContentService>();
            services.AddTransient<ParticipantAssignmentTrackService>();
            services.AddTransient<CourseCriteriaService>();
            services.AddTransient<BrokenLinkService>();
            services.AddTransient<AnnouncementService>();
            services.AddTransient<BlockoutDateService>();
            services.AddTransient<AssessmentAnswerService>();

            // Infrastructure
            services.AddTransient(typeof(IAccessControlContext<>), typeof(GenericAccessControlContext<>));
            services.AddSingleton<HttpService>();
            services.AddSingleton<IAuthenticationTokenService, AuthenticationTokenService>();
            services.AddTransient<IStorageService, StorageService>();

            services.AddTransient<IMetadataSynchronizer, MetadataSynchronizer>();
            services.AddTransient<IMetadataDataProvider, CourseMetadataDataProvider>();

            services.AddSingleton<IAuditLogConventions>(provider => new AuditLogConventions(new Dictionary<AuditLogActionType, string[]>
            {
                { AuditLogActionType.Created, new[] { "Clone", "Create", "Nominate", "Add" } },
                { AuditLogActionType.Updated, new[] { "Assign", "Change", "Move", "Save", "Init", "Update", "Transfer", "Set", "Mark", "Complete" } },
                { AuditLogActionType.Deleted, new[] { "Delete" } }
            }));
            services.AddSingleton<IValidFilterCriteria>(provider => new ValidFilterCriteria(new Dictionary<string, ImmutableHashSet<string>>
            {
                {
                    nameof(CourseEntity),
                    ImmutableHashSet.Create(
                        nameof(CourseEntity.CreatedBy),
                        nameof(CourseEntity.DepartmentId),
                        nameof(CourseEntity.CreatedDate),
                        nameof(CourseEntity.ChangedDate),
                        nameof(CourseEntity.Status),
                        nameof(CourseEntity.ContentStatus),
                        nameof(CourseEntity.PDActivityType),
                        nameof(CourseEntity.CategoryIds),
                        nameof(CourseEntity.ServiceSchemeIds),
                        nameof(CourseEntity.DevelopmentalRoleIds),
                        nameof(CourseEntity.TeachingLevels),
                        nameof(CourseEntity.CourseLevel),
                        nameof(CourseEntity.SubjectAreaIds),
                        nameof(CourseEntity.LearningFrameworkIds),
                        nameof(CourseEntity.LearningDimensionIds),
                        nameof(CourseEntity.LearningAreaIds),
                        nameof(CourseEntity.LearningSubAreaIds))
                },
                {
                    nameof(Announcement),
                    ImmutableHashSet.Create(
                        nameof(Announcement.CreatedBy))
                },
                {
                    nameof(ClassRun),
                    ImmutableHashSet.Create(
                        nameof(ClassRun.Status),
                        nameof(ClassRun.ContentStatus))
                },
                {
                    nameof(Registration),
                    ImmutableHashSet.Create(
                        nameof(Registration.Status),
                        nameof(Registration.WithdrawalStatus),
                        nameof(Registration.ClassRunChangeStatus))
                },
                {
                    nameof(CourseUser),
                    ImmutableHashSet.Create(
                        nameof(CourseUser.DepartmentId),
                        nameof(CourseUser.ServiceScheme),
                        nameof(CourseUser.Designation),
                        nameof(CourseUser.DevelopmentalRole),
                        nameof(CourseUser.TeachingLevel),
                        nameof(CourseUser.TeachingCourseOfStudy),
                        nameof(CourseUser.TeachingSubject),
                        nameof(CourseUser.LearningFramework))
                },
                {
                    nameof(BlockoutDate),
                    ImmutableHashSet.Create(
                        nameof(BlockoutDate.StartDate),
                        nameof(BlockoutDate.EndDate),
                        nameof(BlockoutDate.ServiceSchemes))
                }
            }));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditLogBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryTrackingBehavior<,>));

            // Inject HangFire Job
            services.AddTransient<IAttendanceCheckingJob, AttendanceCheckingJob>();
            services.AddTransient<IOfferExpirationCheckingJob, OfferExpirationCheckingJob>();
            services.AddTransient<ICourseDailyCheckingJob, CourseDailyCheckingJob>();
            services.AddTransient<IContentDailyCheckingJob, ContentDailyCheckingJob>();
            services.AddTransient<IRemindTakingAttendanceJob, RemindTakingAttendanceJob>();
            services.AddTransient<IAssignmentDailyCheckingJob, AssignmentDailyCheckingJob>();
            services.AddTransient<IAnnouncementCheckingJob, AnnouncementCheckingJob>();
            services.AddTransient<ICoursePlanningCycleCheckingJob, CoursePlanningCycleCheckingJob>();
            services.AddTransient<IAssignmentExtendedCheckingJob, AssignmentExtendedCheckingJob>();
            services.AddTransient<IClassrunRegistrationDailyCheckingJob, ClassrunRegistrationDailyCheckingJob>();
            /*
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());
            */

            services.AddTransient<WebAppLinkBuilder>();
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
