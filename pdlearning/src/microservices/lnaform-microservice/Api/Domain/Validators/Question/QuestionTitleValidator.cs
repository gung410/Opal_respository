using FluentValidation;
using QuestionEntity = Microservice.LnaForm.Domain.ValueObjects.Questions.Question;

namespace Microservice.LnaForm.Domain.Validators.Question
{
    public class QuestionTitleValidator : AbstractValidator<string>
    {
        public QuestionTitleValidator()
        {
            RuleFor(p => p)
                .MaximumLength(20000)
                .NotEmpty();
        }
    }
}
