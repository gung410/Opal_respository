using FluentValidation;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Domain.Validators.FormQuestionAnswer;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.Validators
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
