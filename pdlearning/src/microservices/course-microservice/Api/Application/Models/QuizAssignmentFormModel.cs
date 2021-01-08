using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class QuizAssignmentFormModel
    {
        public QuizAssignmentFormModel(QuizAssignmentForm quizAssignmentForm, IEnumerable<QuizAssignmentFormQuestion> quizAssignmentFormQuestions, bool forLearnerAnswer)
        {
            Id = quizAssignmentForm.Id;
            RandomizedQuestions = quizAssignmentForm.RandomizedQuestions;
            Questions = quizAssignmentFormQuestions?.OrderBy(p => p.Priority).Select(x => new QuizAssignmentFormQuestionModel(x, forLearnerAnswer));
        }

        public Guid? Id { get; set; }

        public bool RandomizedQuestions { get; set; }

        public IEnumerable<QuizAssignmentFormQuestionModel> Questions { get; set; }
    }
}
