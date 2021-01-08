using FluentValidation;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Validators.CommunityEvents
{
    public class CreateCommunityEventValidator : AbstractValidator<CreateCommunityEventRequest>
    {
        public CreateCommunityEventValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.CommunityId).NotEmpty();
            RuleFor(x => x.StartAt).LessThan(x => x.EndAt);
        }
    }
}
