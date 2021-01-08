namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyQuestionAnswerStatisticsModel
    {
        public SurveyQuestionAnswerStatisticsModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public int AnswerCode { get; set; }

        public string AnswerValue { get; set; }

        public double AnswerCount { get; set; }

        public double AnswerPercentage { get; set; }
    }
}
