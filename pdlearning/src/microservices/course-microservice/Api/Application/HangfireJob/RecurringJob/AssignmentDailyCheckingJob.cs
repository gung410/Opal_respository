using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class AssignmentDailyCheckingJob : BaseHangfireJob, IAssignmentDailyCheckingJob
    {
        private readonly OpalSettingsOption _opalSettingsOption;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;

        public AssignmentDailyCheckingJob(
            IOptions<OpalSettingsOption> opalSettingsOption,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _opalSettingsOption = opalSettingsOption.Value;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
        }

        protected override async Task InternalHandleAsync()
        {
            await NotifyLearnerOnAssignmentDueDates();
            await ThunderCqrs.SendCommand(new UpdateIncompleteParticipantAssignmentTrackCommand());
            await ThunderCqrs.SendCommand(new AutoSubmitOverDueDateAssignmentCommand());
        }

        private async Task NotifyLearnerOnAssignmentDueDates()
        {
            // Get all assigned participant assignment must be finished after [BeforeAssignmentDeadlineDays] days.
            var mustBeFinishedSoonParticipantAssignmentTracks = await _getAggregatedParticipantAssignmentTrackSharedQuery
                .All(x => x.EndDate.Date == Clock.Now.Date.AddDays(_opalSettingsOption.BeforeAssignmentDeadlineDays));

            var aggregatedRegistrations =
                (await _getAggregatedRegistrationSharedQuery
                    .WithClassAndCourseByRegistrationIds(mustBeFinishedSoonParticipantAssignmentTracks.Select(x => x.ParticipantAssignmentTrack.RegistrationId).ToList()))
                .ToDictionary(p => p.Registration.Id);

            var events = mustBeFinishedSoonParticipantAssignmentTracks
                .Select(p =>
                {
                    var courseTitle = aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId)
                        ? aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].Course.CourseName
                        : string.Empty;
                    var classRunTitle = aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId)
                        ? aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].ClassRun.ClassTitle
                        : string.Empty;
                    return new AssignmentDueDateNotifyLearnerEvent(
                        Guid.Empty,
                        new AssignmentDueDateNotifyLearnerPayload
                        {
                            CourseTitle = courseTitle,
                            ClassRunTitle = classRunTitle,
                            DueDate = TimeHelper.ConvertTimeFromUtc(p.ParticipantAssignmentTrack.EndDate).ToString(DateTimeFormatConstant.OnlyDate),
                            DaysBeforeAssignmentDeadline =
                                _opalSettingsOption.BeforeAssignmentDeadlineDays.ToString(),
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(
                                aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId)
                                    ? aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].Course.Id
                                    : Guid.Empty),
                            AssignmentTile = p.Assignment.Title
                        },
                        new List<Guid> { p.ParticipantAssignmentTrack.UserId });
                });

            await ThunderCqrs.SendEvents(events);
        }
    }
}
