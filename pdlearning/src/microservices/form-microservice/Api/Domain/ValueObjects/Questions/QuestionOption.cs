using System;
using Microservice.Form.Application.Models;

namespace Microservice.Form.Domain.ValueObjects.Questions
{
    public class QuestionOption : BaseValueObject
    {
        public QuestionOption()
        {
        }

        public QuestionOption(int code, object value, string feedback = null, QuestionOptionType? type = null, string imageUrl = null, string videoUrl = null, Guid? nextQuestionId = null)
        {
            Code = code;
            Value = value;
            Feedback = feedback;
            Type = type;
            ImageUrl = imageUrl;
            VideoUrl = videoUrl;
            NextQuestionId = nextQuestionId;
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public string Feedback { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public bool IsEmptyValue { get; set; }

        // This field for analytic rubric form only
        public Guid? ScaleId { get; set; }

        /// <summary>
        /// Determine the next question based on the option.
        /// </summary>
        public Guid? NextQuestionId { get; set; }

        /// <summary>
        /// Map to <see cref="QuestionOptionModel"/>.
        /// </summary>
        /// <param name="questionOption">Question Option.</param>
        public static implicit operator QuestionOptionModel(QuestionOption questionOption) => ToQuestionOptionModel(questionOption);

        public static explicit operator QuestionOption(QuestionOptionModel questionOptionModel) => ToQuestionOption(questionOptionModel);

        public static QuestionOptionModel ToQuestionOptionModel(QuestionOption questionOption) => new QuestionOptionModel
        {
            Code = questionOption.Code,
            Value = questionOption.Value,
            Feedback = questionOption.Feedback,
            Type = questionOption.Type,
            ImageUrl = questionOption.ImageUrl,
            VideoUrl = questionOption.VideoUrl,
            NextQuestionId = questionOption.NextQuestionId,
            IsEmptyValue = questionOption.IsEmptyValue,
            ScaleId = questionOption.ScaleId
        };

        public static QuestionOption ToQuestionOption(QuestionOptionModel questionOptionModel) => new QuestionOption
        {
            Code = questionOptionModel.Code,
            Value = questionOptionModel.Value,
            Feedback = questionOptionModel.Feedback,
            Type = questionOptionModel.Type,
            ImageUrl = questionOptionModel.ImageUrl,
            VideoUrl = questionOptionModel.VideoUrl,
            NextQuestionId = questionOptionModel.NextQuestionId,
            IsEmptyValue = questionOptionModel.IsEmptyValue,
            ScaleId = questionOptionModel.ScaleId
        };
    }
}
