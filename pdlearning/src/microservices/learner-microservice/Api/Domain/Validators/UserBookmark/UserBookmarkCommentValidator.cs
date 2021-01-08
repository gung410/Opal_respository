using FluentValidation;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Domain.Validators
{
    public class UserBookmarkCommentValidator : AbstractValidator<string>
    {
        public UserBookmarkCommentValidator()
        {
            RuleFor(p => p).MaximumLength(UserBookmark.MaxCommentLength);
        }
    }
}
