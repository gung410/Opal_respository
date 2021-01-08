namespace LearnerApp.Models.MyLearning
{
    public class QuizData
    {
        public string CourseId { get; set; }

        public string QuizFormId { get; set; }

        public string MyCourseId { get; set; }

        public AdditionalInfo AdditionalInfo { get; set; }

        public bool IsViewAgain { get; set; }
    }
}
