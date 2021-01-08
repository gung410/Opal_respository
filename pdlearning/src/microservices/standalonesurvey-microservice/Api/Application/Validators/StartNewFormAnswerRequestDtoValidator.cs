using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class StartNewFormAnswerRequestDtoValidator : AbstractValidator<SaveSurveyAnswerRequestDto>
    {
        public StartNewFormAnswerRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
