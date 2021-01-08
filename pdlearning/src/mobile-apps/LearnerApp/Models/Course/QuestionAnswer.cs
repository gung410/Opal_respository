using System;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class QuestionAnswer : BaseViewModel
    {
        public int Id { get; set; }

        public string QuizAssignmentFormQuestionId { get; set; }

        public string QuizAnswerId { get; set; }

        public object AnswerValue { get; set; }

        public int Score { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}
