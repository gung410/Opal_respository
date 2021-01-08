using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;

namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class GetFormWithQuestionsByIdRequestDtoValidator : AbstractValidator<GetSurveyWithQuestionsByIdRequestDto>
    {
        public GetFormWithQuestionsByIdRequestDtoValidator()
        {
            RuleFor(p => p.FormId).NotEmpty();
        }
    }
}
