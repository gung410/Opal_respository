using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
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
    public class CourseDailyCheckingJob : BaseHangfireJob, ICourseDailyCheckingJob
    {
        private readonly GetCoursesSharedQuery _getCoursesSharedQuery;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly BookWebinarMeetingLogic _bookingWebinarMeetingLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly OpalSettingsOption _opalSettingsOption;

        public CourseDailyCheckingJob(
            IThunderCqrs thunderCqrs,
            GetCoursesSharedQuery getCoursesSharedQuery,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            BookWebinarMeetingLogic bookingWebinarMeetingLogic,
            WebAppLinkBuilder webAppLinkBuilder,
            IOptions<OpalSettingsOption> opalSettingsOption,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _getCoursesSharedQuery = getCoursesSharedQuery;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseRepository = readCourseRepository;
            _readSessionRepository = readSessionRepository;
            _bookingWebinarMeetingLogic = bookingWebinarMeetingLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task InternalHandleAsync()
        {
            await ByPassTaskException(ChangeCourseStatusToCompleted, nameof(ChangeCourseStatusToCompleted));
            await ByPassTaskException(ChangeCourseStatusToPublished, nameof(ChangeCourseStatusToPublished));
            await ByPassTaskException(NotifyAlternativeApprovingOfficer, nameof(NotifyAlternativeApprovingOfficer));
            await ByPassTaskException(ArchiveCourses, nameof(ArchiveCourses));
            await ByPassTaskException(NotifyRemindConfirmRegistrations, nameof(NotifyRemindConfirmRegistrations));

            // TODO: Should remove this after ISessionDailyCheckingJob is created from SAM. This is temporarily because
            // we do not have chance to create interface for this job
            await ByPassTaskException(
                () => SessionDailyCheckingJob.HandleAsync(_readSessionRepository, _opalSettingsOption, _bookingWebinarMeetingLogic),
                nameof(SessionDailyCheckingJob));
        }

        private async Task ChangeCourseStatusToCompleted()
        {
            var expiredCourses = await _getCoursesSharedQuery.WithExpiredCourses();

            if (expiredCourses.Any())
            {
                await ThunderCqrs.SendCommand(
                    new ChangeCourseStatusCommand()
                    {
                        Ids = expiredCourses.Select(p => p.Id).ToList(),
                        Status = CourseStatus.Completed
                    });
            }
        }

        private async Task ChangeCourseStatusToPublished()
        {
            var notPublishedCourses = await _getCoursesSharedQuery.WithNotPublishedCourses();

            if (notPublishedCourses.Any())
            {
                await ThunderCqrs.SendCommand(
                    new ChangeCourseStatusCommand
                    {
                        Ids = notPublishedCourses.Select(p => p.Id).ToList(),
                        Status = CourseStatus.Published
                    });
            }
        }

        private async Task NotifyAlternativeApprovingOfficer()
        {
            var pendingApprovalCourses = await _readCourseRepository.GetAll()
                .Where(x => x.AlternativeApprovingOfficerId.HasValue)
                .Where(CourseEntity.PendingApprovalSubmittedDaysAgoFromNow(_opalSettingsOption.NotifyPendingApprovalCourseSubmittedDaysAgo))
                .Select(x => new { x.Id, x.AlternativeApprovingOfficerId })
                .ToListAsync();

            var notifyApproverEvent = pendingApprovalCourses.Select(course =>
                new UpdatedCourseNotifyApproverEvent(
                        Guid.Empty,
                        new UpdatedCourseNotifyApproverPayload
                        {
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                                                    CAMTabConfigurationConstant.HasCoursePendingApprovalTab,
                                                                    CAMTabConfigurationConstant.CourseInfoTab,
                                                                    CAMTabConfigurationConstant.AllClassRunsTab,
                                                                    CourseDetailModeConstant.ForApprover,
                                                                    course.Id),
                        },
                        new List<Guid> { course.AlternativeApprovingOfficerId.GetValueOrDefault() }));

            await ThunderCqrs.SendEvents(notifyApproverEvent);
        }

        private async Task ArchiveCourses()
        {
            await ThunderCqrs.SendCommand(new AutoArchiveCoursesCommand());
        }

        private async Task NotifyRemindConfirmRegistrations()
        {
            var canRemindAggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.FullByQuery(
                _readRegistrationRepository.GetAll().Where(Registration.IsPendingConfirmationExpr()),
                ClassRun.IsNotCancelledExpr().AndAlso(ClassRun.GetCanRemindConfirmRegistrationsExpr()));

            var actionLink = _webAppLinkBuilder.GetCourseManagementPageCAMModule(CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab);
            var events = canRemindAggregatedRegistrations
                .Select(aggregatedRegistration =>
                    new RemindConfirmRegistrationsNotifyEvent(
                        Guid.Empty,
                        new RemindConfirmRegistrationsNotifyEventPayload
                        {
                            CourseTitle = aggregatedRegistration.Course.CourseName,
                            ClassrunTitle = aggregatedRegistration.ClassRun.ClassTitle,
                            ActionUrl = actionLink
                        },
                        aggregatedRegistration.ClassRun.GetCanRemindConfirmRegistrationsUserIds(aggregatedRegistration.Course)));

            await ThunderCqrs.SendEvents(events);
        }
    }
}
