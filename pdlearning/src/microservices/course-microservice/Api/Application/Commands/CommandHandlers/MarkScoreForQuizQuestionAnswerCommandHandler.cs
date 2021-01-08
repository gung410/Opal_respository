using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class MarkScoreForQuizQuestionAnswerCommandHandler : BaseCommandHandler<MarkScoreForQuizQuestionAnswerCommand>
    {
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionRepository;
        private readonly IReadOnlyRepository<Assignment> _readAssignmentRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;

        public MarkScoreForQuizQuestionAnswerCommandHandler(
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionRepository,
            IReadOnlyRepository<Assignment> readAssignmentRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _readQuizAssignmentFormQuestionRepository = readQuizAssignmentFormQuestionRepository;
            _readAssignmentRepository = readAssignmentRepository;
            _readCourseRepository = readCourseRepository;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
        }

        protected override async Task HandleAsync(MarkScoreForQuizQuestionAnswerCommand command, CancellationToken cancellationToken)
        {
            var participantAssignmentTrack = await _readParticipantAssignmentTrackRepository.GetAsync(command.ParticipantAssignmentTrackId);
            var assignment = await _readAssignmentRepository.GetAsync(participantAssignmentTrack.AssignmentId);
            var course = await _readCourseRepository.GetAsync(assignment.CourseId);

            EnsureBusinessLogicValid(course.ValidateNotArchived());

            EnsureValidPermission(
                course.HasMarkScorePermission(CurrentUserId, CurrentUserRoles, _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext), p => HasPermissionPrefix(p)));

            var allQuizQuestions = _readQuizAssignmentFormQuestionRepository
                .GetAll()
                .Where(x => x.QuizAssignmentFormId == participantAssignmentTrack.AssignmentId)
                .ToList();

            var allQuizQuestionsDic = allQuizQuestions.ToDictionary(p => p.Id);
            command.QuestionAnswerScores.ForEach(questionAnswerScore =>
            {
                EnsureEntityFound(allQuizQuestionsDic.GetValueOrDefault(questionAnswerScore.QuizAssignmentFormQuestionId, null));

                EnsureBusinessLogicValid(allQuizQuestionsDic[questionAnswerScore.QuizAssignmentFormQuestionId], p => p.ValidateCanMarkScoreForQuestionAnswer(questionAnswerScore.Score));
            });

            participantAssignmentTrack.QuizAnswer.MarkManualScore(
                command.QuestionAnswerScores.Select(p => new ParticipantAssignmentTrackQuizQuestionAnswer_ScoreInfo
                {
                    QuizAssignmentFormQuestionId = p.QuizAssignmentFormQuestionId,
                    Score = p.Score,
                    ManualScoredBy = CurrentUserId
                }).ToList(),
                allQuizQuestions);

            await _participantAssignmentTrackCudLogic.Update(participantAssignmentTrack, cancellationToken);
        }
    }
}
