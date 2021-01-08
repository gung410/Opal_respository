using FluentValidation;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microservice.Form.Domain.Validators.FormQuestionAnswer;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.Validators
{
    public class UpdateFormAnswerRequestDtoValidator : AbstractValidator<UpdateFormAnswerRequestDto>
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
            RuleFor(p => p.SpentTimeInSeconds).SetValidator(new FormQuestionAnswerSpentTimeInSecondsValidator());
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
