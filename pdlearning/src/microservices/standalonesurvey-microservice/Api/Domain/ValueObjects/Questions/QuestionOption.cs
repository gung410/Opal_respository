using System;
using Microservice.StandaloneSurvey.Application.Models;

namespace Microservice.StandaloneSurvey.Domain.ValueObjects.Questions
{
    public class QuestionOption : BaseValueObject
    {
        public QuestionOption()
        {
        }

        public QuestionOption(int code, object value, QuestionOptionType? type = null, string imageUrl = null, string videoUrl = null, Guid? nextQuestionId = null)
        {
            Code = code;
            Value = value;
            Type = type;
            ImageUrl = imageUrl;
            VideoUrl = videoUrl;
            NextQuestionId = nextQuestionId;
        }

        public int Code { get; set; }

        public object Value { get; set; }

        public QuestionOptionType? Type { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public bool IsEmptyValue { get; set; }

        /// <summary>
        /// Determine the next question based on the option.
        /// </summary>
        public Guid? NextQuestionId { get; set; }

        /// <summary>
        /// Map to <see cref="QuestionOptionModel"/>.
        /// </summary>
        /// <param name="questionOption">Question Option.</param>
        public static implicit operator QuestionOptionModel(QuestionOption questionOption) =>
            ToQuestionOptionModel(questionOption);

        public static explicit operator QuestionOption(QuestionOptionModel questionOptionModel) =>
            ToQuestionOption(questionOptionModel);

        public static QuestionOptionModel ToQuestionOptionModel(QuestionOption questionOption) => new QuestionOptionModel
        {
            Code = questionOption.Code,
            Value = questionOption.Value,
            Type = questionOption.Type,
            ImageUrl = questionOption.ImageUrl,
            VideoUrl = questionOption.VideoUrl,
            NextQuestionId = questionOption.NextQuestionId,
            IsEmptyValue = questionOption.IsEmptyValue
        };

        public static QuestionOption ToQuestionOption(QuestionOptionModel questionOptionModel) => new QuestionOption
        {
            Code = questionOptionModel.Code,
            Value = questionOptionModel.Value,
            Type = questionOptionModel.Type,
            ImageUrl = questionOptionModel.ImageUrl,
            VideoUrl = questionOptionModel.VideoUrl,
            NextQuestionId = questionOptionModel.NextQuestionId,
            IsEmptyValue = questionOptionModel.IsEmptyValue
        };
    }
}
