using System.Collections.Generic;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class QuizAnswer : BaseViewModel
    {
        public string Id { get; set; }

        public string QuizAssignmentFormId { get; set; }

        public int Score { get; set; }

        public double ScorePercentage { get; set; }

        public IList<QuestionAnswer> QuestionAnswers { get; set; }
    }
}
