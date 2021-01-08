namespace LearnerApp.Models
{
    public enum LectureType
    {
        InlineContent,
        DigitalContent,
        Quiz,
        None
    }

    public class AdditionalInfo
    {
        public LectureType Type { get; set; }

        public string ResourceId { get; set; }

        public string Value { get; set; }

        public QuizConfig QuizConfig { get; set; }
    }
}
