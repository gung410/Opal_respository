using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.Commands;
using Microservice.Course.Common.Helpers;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.RequestDtos
{
    public class SaveAssignmentQuizAnswerRequest
    {
        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public List<SaveAssignmentQuizAnswerRequestQuestionAnswer> QuestionAnswers { get; set; }

        public bool IsSubmit { get; set; }

        public SaveAssignmentQuizAnswerCommand ToCommand()
        {
            return new SaveAssignmentQuizAnswerCommand
            {
                AssignmentId = AssignmentId,
                RegistrationId = RegistrationId,
                QuestionAnswers = QuestionAnswers?.Select(p => p.ToCommand()) ?? F.List<SaveAssignmentQuizAnswerCommandQuestionAnswer>(),
                IsSubmit = IsSubmit
            };
        }
    }

    public class SaveAssignmentQuizAnswerRequestQuestionAnswer
    {
        public Guid QuizAssignmentFormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public SaveAssignmentQuizAnswerCommandQuestionAnswer ToCommand()
        {
            return new SaveAssignmentQuizAnswerCommandQuestionAnswer
            {
                QuizAssignmentFormQuestionId = QuizAssignmentFormQuestionId,
                AnswerValue = AnswerValue
            };
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
