using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class AutoSubmitOverDueDateAssignmentCommandHandler : BaseCommandHandler<AutoSubmitOverDueDateAssignmentCommand>
    {
        private readonly IReadOnlyRepository<QuizAssignmentFormQuestion> _readQuizAssignmentFormQuestionsRepository;
        private readonly ParticipantAssignmentTrackCudLogic _participantAssignmentTrackCudLogic;
        private readonly GetAggregatedParticipantAssignmentTrackSharedQuery _getAggregatedParticipantAssignmentTrackSharedQuery;
        private readonly SendSubmitAssignmentNotificationLogic _sendSubmitAssignmentNotificationLogic;
        private readonly OpalSettingsOption _opalSettingsOption;

        public AutoSubmitOverDueDateAssignmentCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IReadOnlyRepository<QuizAssignmentFormQuestion> readQuizAssignmentFormQuestionsRepository,
            GetAggregatedParticipantAssignmentTrackSharedQuery getAggregatedParticipantAssignmentTrackSharedQuery,
            SendSubmitAssignmentNotificationLogic sendSubmitAssignmentNotificationLogic,
            IOptions<OpalSettingsOption> opalSettingsOption,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readQuizAssignmentFormQuestionsRepository = readQuizAssignmentFormQuestionsRepository;
            _participantAssignmentTrackCudLogic = participantAssignmentTrackCudLogic;
            _getAggregatedParticipantAssignmentTrackSharedQuery = getAggregatedParticipantAssignmentTrackSharedQuery;
            _sendSubmitAssignmentNotificationLogic = sendSubmitAssignmentNotificationLogic;
            _opalSettingsOption = opalSettingsOption.Value;
        }

        protected override async Task HandleAsync(AutoSubmitOverDueDateAssignmentCommand command, CancellationToken cancellationToken)
        {
            var afterAssignmentExtendedDays = _opalSettingsOption.AssignmentExtendedDays + 1;
            var aggregatedParticipantAssignmentTracks = await _getAggregatedParticipantAssignmentTrackSharedQuery
                .All(p => p.SubmittedDate == null && p.EndDate.AddDays(afterAssignmentExtendedDays).Date == Clock.Now.Date);

            var assignmentIds = aggregatedParticipantAssignmentTracks.Select(x => x.ParticipantAssignmentTrack.AssignmentId);

            var assignmentFormQuestions = await _readQuizAssignmentFormQuestionsRepository
                .GetAll()
                .Where(p => assignmentIds.Contains(p.QuizAssignmentFormId))
                .ToListAsync(cancellationToken);

            var assignmentFormQuestionDic = assignmentFormQuestions
                .GroupBy(x => x.QuizAssignmentFormId)
                .ToDictionary(x => x.Key, x => x.Where(c => c != null).ToList());

            aggregatedParticipantAssignmentTracks.ForEach(p =>
            {
                if (assignmentFormQuestionDic.ContainsKey(p.ParticipantAssignmentTrack.AssignmentId))
                {
                    p.ParticipantAssignmentTrack.UpdateSubmittedDate(Clock.Now, _opalSettingsOption.AssignmentExtendedDays);

                    if (p.ParticipantAssignmentTrack.QuizAnswer == null)
                    {
                        p.ParticipantAssignmentTrack.QuizAnswer = new ParticipantAssignmentTrackQuizAnswer()
                        {
                            Id = p.ParticipantAssignmentTrack.Id,
                            QuizAssignmentFormId = p.ParticipantAssignmentTrack.AssignmentId
                        };
                    }
                    else
                    {
                        p.ParticipantAssignmentTrack.QuizAnswer.UpdateQuestionAnswerValues(
                            p.ParticipantAssignmentTrack.QuizAnswer.QuestionAnswers.ToList(),
                            assignmentFormQuestionDic.GetValueOrDefault(p.ParticipantAssignmentTrack.AssignmentId, new List<QuizAssignmentFormQuestion>()),
                            true);
                    }
                }
            });

            await _participantAssignmentTrackCudLogic.UpdateMany(
                aggregatedParticipantAssignmentTracks.Select(p => p.ParticipantAssignmentTrack).ToList(),
                cancellationToken);

            await _sendSubmitAssignmentNotificationLogic.Execute(aggregatedParticipantAssignmentTracks, cancellationToken);
        }
    }
}
