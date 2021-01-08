using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Domain.ValueObjects
{
    public class QuestionOption : BaseValueObject
    {
        public QuestionOption()
        {
        }

        public QuestionOption(int code, object value, string feedback = null, QuestionOptionType? type = null, string imageUrl = null)
        {
            Code = code;
            Value = value;
            Feedback = feedback;
            Type = type;
            ImageUrl = imageUrl;
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public string Feedback { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public QuestionOption Clone()
        {
            return (QuestionOption)MemberwiseClone();
        }
    }
}
