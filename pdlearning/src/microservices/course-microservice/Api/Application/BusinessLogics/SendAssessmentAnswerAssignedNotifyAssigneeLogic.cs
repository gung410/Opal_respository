using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendAssessmentAnswerAssignedNotifyAssigneeLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;

        public SendAssessmentAnswerAssignedNotifyAssigneeLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readUserRepository = readUserRepository;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
        }

        public async Task Execute(
            Guid currentUserId,
            List<AssessmentAnswer> assessmentAnswers,
            CancellationToken cancellationToken = default)
        {
            var assignor = await _readUserRepository.GetAsync(currentUserId);

            var participantAssignmentTrackQuery = _readParticipantAssignmentTrackRepository
                .GetAll()
                .Where(p => assessmentAnswers.Select(a => a.ParticipantAssignmentTrackId).Contains(p.Id));

            var participantAssignmentTracksAggregated = (await _getAggregatedParticipantAssignmentTrackSharedQuery
                .FullByQuery(participantAssignmentTrackQuery))
                .Join(
                    assessmentAnswers,
                    aggregated => aggregated.ParticipantAssignmentTrack.Id,
                    assessmentAnswer => assessmentAnswer.ParticipantAssignmentTrackId,
                    (aggregated, assessmentAnswer) => new { aggregated, assessmentAnswer });

            var notifyLearnersEvent = participantAssignmentTracksAggregated
                .SelectList(
                    x =>
                    new AssessmentAnswerAssignedNotifyAssigneeEvent(
                        currentUserId,
                        new AssessmentAnswerAssignedNotifyAssigneePayload
                        {
                            CourseTitle = x.aggregated.Course.CourseName,
                            ClassRunTitle = x.aggregated.ClassRun.ClassTitle,
                            AssignmentName = x.aggregated.Assignment.Title,
                            AssignorName = assignor.FullName(),
                            LearnerName = x.aggregated.Learner.FullName()
                        },
                        new List<Guid> { x.assessmentAnswer.UserId }));

            await _thunderCqrs.SendEvents(notifyLearnersEvent, cancellationToken);
        }
    }
}
