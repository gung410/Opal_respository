using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveAssignmentAnswerQuizCommandHandler : BaseCommandHandler<SaveAssignmentQuizAnswerCommand>
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionsRepository;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;
        private readonly SendSubmitAssignmentNotificationLogic _sendSubmitAssignmentNotificationLogic;
        private readonly OpalSettingsOption _opalSettingsOption;

        public SaveAssignmentAnswerQuizCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionsRepository,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic,
            SendSubmitAssignmentNotificationLogic sendSubmitAssignmentNotificationLogic,
            IOptions<OpalSettingsOption> opalSettingsOption) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readQuizAssignmentFormQuestionsRepository = readQuizAssignmentFormQuestionsRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
            _sendSubmitAssignmentNotificationLogic = sendSubmitAssignmentNotificationLogic;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task HandleAsync(SaveAssignmentQuizAnswerCommand command, CancellationToken cancellationToken)
        {
            var aggregatedParticipantAssignmentTrack = (await _getAggregatedParticipantAssignmentTrackSharedQuery
                .FullByQuery(
                    _readParticipantAssignmentTrackRepository.GetAll().Where(
                        p => p.AssignmentId == command.AssignmentId && p.RegistrationId == command.RegistrationId),
                    cancellationToken))
                .FirstOrDefault();

            aggregatedParticipantAssignmentTrack = EnsureEntityFound(aggregatedParticipantAssignmentTrack);
            EnsureValidPermission(aggregatedParticipantAssignmentTrack.ParticipantAssignmentTrack.HasSaveAnswerPermission(CurrentUserId));
            EnsureBusinessLogicValid(aggregatedParticipantAssignmentTrack.Course.ValidateNotArchived());

            var participantAssignmentTrack = aggregatedParticipantAssignmentTrack.ParticipantAssignmentTrack;
            var assignment = await _readAssignmentRepository.GetAsync(participantAssignmentTrack.AssignmentId);

            if (command.IsSubmit)
            {
                participantAssignmentTrack.UpdateSubmittedDate(Clock.Now, _opalSettingsOption.AssignmentExtendedDays);
            }

            var assignmentFormQuestions = await _readQuizAssignmentFormQuestionsRepository
                .GetAll()
                .Where(p => p.QuizAssignmentFormId == participantAssignmentTrack.AssignmentId)
                .ToListAsync(cancellationToken);

            if (participantAssignmentTrack.QuizAnswer == null)
            {
                participantAssignmentTrack.QuizAnswer = new ParticipantAssignmentTrackQuizAnswer() { Id = participantAssignmentTrack.Id, QuizAssignmentFormId = participantAssignmentTrack.AssignmentId };
            }

            if (command.QuestionAnswers != null)
            {
                participantAssignmentTrack.QuizAnswer.UpdateQuestionAnswerValues(
                    command.QuestionAnswers.Select(p => p.ToEntity(participantAssignmentTrack.QuizAnswer.Id)).ToList(),
                    assignmentFormQuestions.ToList(),
                    command.IsSubmit);
            }

            await _participantAssignmentTrackCudLogic.Update(participantAssignmentTrack, cancellationToken);

            if (command.IsSubmit)
            {
                await _sendSubmitAssignmentNotificationLogic.Execute(
                    new List<ParticipantAssignmentTrackAggregatedEntityModel>
                    {
                        ParticipantAssignmentTrackAggregatedEntityModel
                        .New()
                        .WithParticipantAssignmentTrack(participantAssignmentTrack)
                        .WithAssignment(assignment)
                    },
                    cancellationToken);
            }
        }
    }
}
