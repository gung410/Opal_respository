using System;
using System.Diagnostics.CodeAnalysis;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class SurveyQuestionAnswer : BaseEntity
    {
        public Guid SurveyAnswerId { get; set; }

        public Guid SurveyQuestionId { get; set; }

        /// <summary>
        /// Answer value is string for short/long text question, boolean for true/false question, List of string for multiple choice, etc..
        /// </summary>
        public object AnswerValue { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }
    }
}
