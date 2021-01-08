using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
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
    public class AssignmentExtendedCheckingJob : BaseHangfireJob, IAssignmentExtendedCheckingJob
    {
        private readonly OpalSettingsOption _opalSettingsOption;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;

        public AssignmentExtendedCheckingJob(
            IOptions<OpalSettingsOption> opalSettingsOption,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            ILoggerFactory loggerFactory) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _opalSettingsOption = opalSettingsOption.Value;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
        }

        protected override async Task InternalHandleAsync()
        {
            await NotifyLearnerOnAssignmentExtended();
        }

        private async Task NotifyLearnerOnAssignmentExtended()
        {
            var afterEndDate = _opalSettingsOption.AssignmentExtendedDays - _opalSettingsOption.BeforeAssignmentExtendedDays;

            var aggregatedParticipantAssignmentTracks = await _getAggregatedParticipantAssignmentTrackSharedQuery
                .All(x => x.EndDate.AddDays(afterEndDate).Date == Clock.Now.Date);

            var aggregatedRegistrations =
                (await _getAggregatedRegistrationSharedQuery
                    .WithClassAndCourseByRegistrationIds(aggregatedParticipantAssignmentTracks.Select(x => x.ParticipantAssignmentTrack.RegistrationId).ToList()))
                .ToDictionary(p => p.Registration.Id);

            var events = aggregatedParticipantAssignmentTracks
                .Select(p =>
                {
                    var courseTitle = aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId)
                        ? aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].Course.CourseName
                        : string.Empty;
                    var classRunTitle = aggregatedRegistrations.ContainsKey(p.ParticipantAssignmentTrack.RegistrationId)
                        ? aggregatedRegistrations[p.ParticipantAssignmentTrack.RegistrationId].ClassRun.ClassTitle
                        : string.Empty;
                    return new AssignmentExtendedNotifyLearnerEvent(
                        Guid.Empty,
                        new AssignmentExtendedNotifyLearnerPayload
                        {
                            CourseTitle = courseTitle,
                            ClassRunTitle = classRunTitle,
                            ExtendedDueDate = TimeHelper
                                .ConvertTimeFromUtc(p.ParticipantAssignmentTrack.EndDate.AddDays(_opalSettingsOption.AssignmentExtendedDays))
                                .ToString(DateTimeFormatConstant.OnlyDate)
                        },
                        new List<Guid> { p.ParticipantAssignmentTrack.UserId });
                });

            await ThunderCqrs.SendEvents(events);
        }
    }
}
