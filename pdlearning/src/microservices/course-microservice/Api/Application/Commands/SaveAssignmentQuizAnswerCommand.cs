using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class SaveAssignmentQuizAnswerCommand : BaseThunderCommand
    {
        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public IEnumerable<SaveAssignmentQuizAnswerCommandQuestionAnswer> QuestionAnswers { get; set; }

        public bool IsSubmit { get; set; }
    }

    public class SaveAssignmentQuizAnswerCommandQuestionAnswer
    {
        public Guid QuizAssignmentFormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public ParticipantAssignmentTrackQuizQuestionAnswer ToEntity(Guid quizAnswerId)
        {
            return new ParticipantAssignmentTrackQuizQuestionAnswer
            {
                QuizAssignmentFormQuestionId = QuizAssignmentFormQuestionId,
                AnswerValue = AnswerValue,
                QuizAnswerId = quizAnswerId
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
