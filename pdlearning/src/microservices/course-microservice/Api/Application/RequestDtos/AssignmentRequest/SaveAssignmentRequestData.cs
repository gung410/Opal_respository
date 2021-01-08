using System;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveAssignmentRequestData
    {
        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public bool RandomizedQuestions { get; set; }

        public AssignmentType Type { get; set; }

        public CreateOrUpdateAssignmentCommandQuizForm QuizForm { get; set; }

        public Guid? AssessmentId { get; set; }

        public ScoreMode? ScoreMode { get; set; }

        public CreateOrUpdateAssignmentCommand ToCreateOrUpdateAssignmentCommand()
        {
            return new CreateOrUpdateAssignmentCommand()
            {
                Id = Id ?? Guid.NewGuid(),
                CourseId = CourseId,
                ClassRunId = ClassRunId,
                Title = Title,
                RandomizedQuestions = RandomizedQuestions,
                Type = Type,
                IsCreated = !Id.HasValue,
                QuizForm = QuizForm,
                AssessmentId = AssessmentId,
                ScoreMode = ScoreMode
            };
        }
    }
}
