using FluentValidation;
using Microservice.Learner.Application.Dtos;

namespace Microservice.Learner.Application.Validators
{
    public class GetMyBookmarkByItemIdsRequestDtoValidator : AbstractValidator<GetMyBookmarkByItemIdsRequestDto>
    {
        public GetMyBookmarkByItemIdsRequestDtoValidator()
        {
            RuleFor(p => p.ItemIds).NotNull();
            RuleFor(p => p.ItemType).NotNull();
        }
    }
}
