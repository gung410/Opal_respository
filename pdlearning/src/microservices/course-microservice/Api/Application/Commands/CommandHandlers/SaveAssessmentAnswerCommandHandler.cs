using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveAssessmentAnswerCommandHandler : BaseCommandHandler<SaveAssessmentAnswerCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<AssessmentAnswer> _readAssessmentAnswerRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;
        private readonly AssessmentAnswerCudLogic _assessmentAnswerCudLogic;

        public SaveAssessmentAnswerCommandHandler(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<AssessmentAnswer> readAssessmentAnswerRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            AssessmentAnswerCudLogic assessmentAnswerCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            IUserContext userContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _thunderCqrs = thunderCqrs;
            _readAssessmentAnswerRepository = readAssessmentAnswerRepository;
            _readUserRepository = readUserRepository;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
            _assessmentAnswerCudLogic = assessmentAnswerCudLogic;
        }

        protected override async Task HandleAsync(SaveAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            var assessmentAnswer = await _readAssessmentAnswerRepository.GetAsync(command.Id);

            assessmentAnswer.CriteriaAnswers = command.CriteriaAnswers;

            if (command.IsSubmit)
            {
                assessmentAnswer.SubmittedDate = Clock.Now;
            }

            await _assessmentAnswerCudLogic.Update(assessmentAnswer, cancellationToken);

            if (command.IsSubmit)
            {
                await SendAssessmentAnswerCompletedNotificationToLearner(assessmentAnswer, cancellationToken);
            }
        }

        private async Task SendAssessmentAnswerCompletedNotificationToLearner(AssessmentAnswer assessmentAnswer, CancellationToken cancellationToken)
        {
            var assessor = CurrentUserId.HasValue ? await _readUserRepository.GetAsync(CurrentUserIdOrDefault) : null;
            var participantAssignmentTrackAggregated = await _getAggregatedParticipantAssignmentTrackSharedQuery.ByParticipantAssignmentTrackId(assessmentAnswer.ParticipantAssignmentTrackId);

            await _thunderCqrs.SendEvent(
                    new AssessmentAnswerCompletedNotifyLearnerEvent(
                        CurrentUserIdOrDefault,
                        new AssessmentAnswerCompletedNotifyLearnerPayload
                        {
                            CourseTitle = participantAssignmentTrackAggregated.Course.CourseName,
                            ClassRunTitle = participantAssignmentTrackAggregated.ClassRun.ClassTitle,
                            AssignmentName = participantAssignmentTrackAggregated.Assignment.Title,
                            AssessorName = assessor.FullName()
                        },
                        new List<Guid> { participantAssignmentTrackAggregated.ParticipantAssignmentTrack.UserId }),
                    cancellationToken);
        }
    }
}
