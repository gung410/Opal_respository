using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class MarkScoreForQuizQuestionAnswerDto
    {
        public float Score { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }

        public MarkScoreForQuizQuestionAnswerCommandQuestionAnswerScore ToCommand()
        {
            return new MarkScoreForQuizQuestionAnswerCommandQuestionAnswerScore()
            {
                ManualScoredBy = ManualScoredBy,
                QuizAssignmentFormQuestionId = QuizAssignmentFormQuestionId,
                Score = Score
            };
        }
    }
}
