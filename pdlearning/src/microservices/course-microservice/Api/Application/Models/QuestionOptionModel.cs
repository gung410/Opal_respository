using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.Models
{
    public class QuestionOptionModel
    {
        public QuestionOptionModel()
        {
        }

        public QuestionOptionModel(QuestionOption questionOption)
        {
            Code = questionOption.Code;
            Value = questionOption.Value;
            Feedback = questionOption.Feedback;
            Type = questionOption.Type;
            ImageUrl = questionOption.ImageUrl;
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public string Feedback { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public QuestionOption ToEntity()
        {
            return new QuestionOption
            {
                Code = Code,
                Value = Value,
                Feedback = Feedback,
                Type = Type,
                ImageUrl = ImageUrl
            };
        }
    }
}
