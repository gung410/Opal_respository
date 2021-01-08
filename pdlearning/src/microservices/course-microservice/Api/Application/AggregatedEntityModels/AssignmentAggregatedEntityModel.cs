using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.AggregatedEntityModels.Abstractions;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.AggregatedEntityModels
{
    public class AssignmentAggregatedEntityModel : BaseAggregatedEntityModel, IAggregatedContentEntityModel
    {
        public Assignment Assignment { get; init; }

        public QuizAssignmentForm QuizForm { get; init; }

        public List<QuizAssignmentFormQuestion> QuizFormQuestions { get; init; } = new List<QuizAssignmentFormQuestion>();

        public CourseUser Owner { get; init; }

        public Guid Id => Assignment.Id;

        public Guid ForTargetId => Assignment.ForTargetId();

        public Guid OwnerId => Assignment.CreatedBy;

        public Guid CourseId => Assignment.CourseId;

        public Guid? ClassRunId => Assignment.ClassRunId;

        public string Title => Assignment.Title;

        public static AssignmentAggregatedEntityModel Create(Assignment assignment, QuizAssignmentForm quizForm, List<QuizAssignmentFormQuestion> quizFormQuestions, CourseUser owner = null)
        {
            return new AssignmentAggregatedEntityModel
            {
                Assignment = assignment,
                QuizForm = quizForm,
                QuizFormQuestions = quizFormQuestions ?? new List<QuizAssignmentFormQuestion>(),
                Owner = owner
            };
        }

        public string CombinedRichText()
        {
            if (QuizFormQuestions != null)
            {
                return string.Join(string.Empty, QuizFormQuestions.Select(_ => _.Question_Title));
            }

            return string.Empty;
        }
    }
}
