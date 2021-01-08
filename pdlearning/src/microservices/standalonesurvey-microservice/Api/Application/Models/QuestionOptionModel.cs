using System;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class QuestionOptionModel
    {
        public QuestionOptionModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsEmptyValue { get; set; }
    }
}
