using System;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Application.Models
{
    public class QuestionOptionModel
    {
        public QuestionOptionModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public string Feedback { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsEmptyValue { get; set; }

        // This field for analytic rubric form only
        public Guid? ScaleId { get; set; }
    }
}
