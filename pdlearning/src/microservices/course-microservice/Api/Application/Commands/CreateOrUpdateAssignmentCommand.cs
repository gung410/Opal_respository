using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class CreateOrUpdateAssignmentCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public bool RandomizedQuestions { get; set; }

        public AssignmentType Type { get; set; }

        public bool IsCreated { get; set; }

        public CreateOrUpdateAssignmentCommandQuizForm QuizForm { get; set; }

        public Guid? AssessmentId { get; set; }

        public ScoreMode? ScoreMode { get; set; }
    }

    public class CreateOrUpdateAssignmentCommandQuizForm
    {
        public bool RandomizedQuestions { get; set; }

        public IEnumerable<QuizAssignmentFormQuestionModel> Questions { get; set; } = new List<QuizAssignmentFormQuestionModel>();
    }
}
#pragma warning restore SA1402 // File may only contain a single type
