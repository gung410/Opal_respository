using FluentValidation;
using QuestionEntity = Microservice.Form.Domain.ValueObjects.Questions.Question;

namespace Microservice.Form.Domain.Validators.Question
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
