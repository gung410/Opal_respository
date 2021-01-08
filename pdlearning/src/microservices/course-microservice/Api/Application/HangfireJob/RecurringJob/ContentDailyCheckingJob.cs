using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class ContentDailyCheckingJob : BaseHangfireJob, IContentDailyCheckingJob
    {
        private readonly GetCoursesSharedQuery _getCoursesSharedQuery;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly OpalSettingsOption _opalSettingsOption;

        public ContentDailyCheckingJob(
            IThunderCqrs thunderCqrs,
            GetCoursesSharedQuery getCoursesSharedQuery,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            WebAppLinkBuilder webAppLinkBuilder,
            IOptions<OpalSettingsOption> opalSettingsOption,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _getCoursesSharedQuery = getCoursesSharedQuery;
            _readCourseRepository = readCourseRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task InternalHandleAsync()
        {
            await ChangeCourseContentStatusToPublished();
            await NotifyAlternativeApprovingOfficer();
        }

        private async Task ChangeCourseContentStatusToPublished()
        {
            var notPublishedCourseContent = await _getCoursesSharedQuery.WithContentNotPublishedCourses();

            if (notPublishedCourseContent.Any())
            {
                await ThunderCqrs.SendCommand(
                    new ChangeCourseContentStatusCommand()
                    {
                        Ids = notPublishedCourseContent.Select(p => p.Id).ToList(),
                        ContentStatus = ContentStatus.Published
                    });
            }
        }

        private async Task NotifyAlternativeApprovingOfficer()
        {
            var pendingApprovalCourseContents = await _readCourseRepository.GetAll()
                 .Where(x => x.AlternativeApprovingOfficerId.HasValue)
                 .Where(CourseEntity.ContentPendingApprovalSubmittedDaysAgoFromNow(_opalSettingsOption.NotifyPendingApprovalContentSubmittedDaysAgo))
                 .Select(x => new { x.Id, x.CourseName, x.AlternativeApprovingOfficerId })
                 .ToListAsync();

            var notifyApproverEvent = pendingApprovalCourseContents.Select(p =>
                new SubmittedCourseContentNotifyApproverEvent(
                    Guid.Empty,
                    new SubmittedCourseContentNotifyApproverPayload
                    {
                        CourseName = p.CourseName,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForLMMModule(
                                            LMMTabConfigurationConstant.PendingApprovalTab,
                                            LMMTabConfigurationConstant.CourseInfoTab,
                                            LMMTabConfigurationConstant.AllClassRunsTab,
                                            CourseDetailModeConstant.ForApprover,
                                            p.Id)
                    },
                    new List<Guid> { p.AlternativeApprovingOfficerId.GetValueOrDefault() }));

            await ThunderCqrs.SendEvents(notifyApproverEvent);
        }
    }
}
