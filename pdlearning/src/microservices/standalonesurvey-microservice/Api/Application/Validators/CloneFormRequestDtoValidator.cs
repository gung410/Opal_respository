using FluentValidation;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Domain.Validators.Survey;

namespace Microservice.StandaloneSurvey.Application.Validators
{
    public class CloneFormRequestDtoValidator : AbstractValidator<CloneSurveyRequestDto>
    {
        public CloneFormRequestDtoValidator()
        {
            RuleFor(p => p.NewTitle)
                .NotNull()
                .SetValidator(new SurveyTitleValidator());
        }
    }
}
