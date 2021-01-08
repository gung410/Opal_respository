using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Domain.Validators.SurveyQuestionAnswer;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class UpdateFormAnswerRequestDtoValidator : AbstractValidator<UpdateSurveyAnswerRequestDto>
    {
        public UpdateFormAnswerRequestDtoValidator()
        {
            RuleForEach(p => p.QuestionAnswers)
                .NotNull()
                .SetValidator(new UpdateFormAnswerRequestDtoQuestionAnswerValidator());
        }
    }

    public class UpdateFormAnswerRequestDtoQuestionAnswerValidator : AbstractValidator<UpdateFormAnswerRequestDtoQuestionAnswer>
    {
        public UpdateFormAnswerRequestDtoQuestionAnswerValidator()
        {
            RuleFor(p => p.SpentTimeInSeconds).SetValidator(new SurveyQuestionAnswerSpentTimeInSecondsValidator());
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
