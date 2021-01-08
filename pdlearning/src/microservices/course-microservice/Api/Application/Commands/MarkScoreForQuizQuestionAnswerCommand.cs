using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class MarkScoreForQuizQuestionAnswerCommand : BaseThunderCommand
    {
        public Guid ParticipantAssignmentTrackId { get; set; }

        public List<MarkScoreForQuizQuestionAnswerCommandQuestionAnswerScore> QuestionAnswerScores { get; set; }
    }

    public class MarkScoreForQuizQuestionAnswerCommandQuestionAnswerScore
    {
        public float Score { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
