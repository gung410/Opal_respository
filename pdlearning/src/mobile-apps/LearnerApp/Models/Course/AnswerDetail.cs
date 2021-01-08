using System.Collections.Generic;
using LearnerApp.ViewModels.Base;

namespace LearnerApp.Models.Course
{
    public class AnswerDetail : BaseViewModel
    {
        public string Id { get; set; }

        public List<QuestionAnswerDetail> QuestionAnswers { get; set; }
    }
}
