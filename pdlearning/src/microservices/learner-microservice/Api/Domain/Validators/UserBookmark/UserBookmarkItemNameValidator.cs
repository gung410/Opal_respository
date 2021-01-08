using FluentValidation;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Domain.Validators
{
    public class UserBookmarkItemNameValidator : AbstractValidator<string>
    {
        public UserBookmarkItemNameValidator()
        {
            RuleFor(p => p).MaximumLength(UserBookmark.MaxItemNameLength);
        }
    }
}
