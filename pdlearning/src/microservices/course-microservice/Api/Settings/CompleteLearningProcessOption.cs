namespace Microservice.Course.Settings
{
    public class CompleteLearningProcessOption
    {
        public const string ConfigurationSectionKey = "CompleteLearningProcessCondition";

        public double AttendanceRatio { get; set; }

        public double LearningContentProgress { get; set; }
    }
}
