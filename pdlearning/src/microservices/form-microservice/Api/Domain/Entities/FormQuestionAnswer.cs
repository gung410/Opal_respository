using System;
using System.Diagnostics.CodeAnalysis;

namespace Microservice.Form.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class FormQuestionAnswer : BaseEntity
    {
        public Guid FormAnswerId { get; set; }

        public Guid FormQuestionId { get; set; }

        /// <summary>
        /// Answer value is string for short/long text question, boolean for true/false question, List of string for multiple choice, etc..
        /// </summary>
        public object AnswerValue { get; set; }

        public double? MaxScore { get; set; }

        public double? Score { get; set; }

        public Guid? ScoredBy { get; set; }

        public Guid? MarkedBy { get; set; }

        public string AnswerFeedback { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }
    }
}