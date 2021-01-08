using FluentValidation;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Domain.Entities;

namespace Microservice.Learner.Application.Validators
{
    public class CreateUserReviewRequestDtoValidator : AbstractValidator<CreateUserReviewRequest>
    {
        public CreateUserReviewRequestDtoValidator()
        {
            RuleFor(p => p.ItemId).NotEmpty();

            // Allow input is Zero because it sames as no choosen rating.
            RuleFor(p => p.Rating).InclusiveBetween(0, 3);
            RuleFor(p => p.CommentContent).MaximumLength(UserReview.MaxCommentContentLength);
        }
    }
}
