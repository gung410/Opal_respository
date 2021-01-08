using FluentValidation;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Validators.CommunityEvents
{
    public class UpdateCommunityEventValidator : AbstractValidator<UpdateCommunityEventRequest>
    {
        public UpdateCommunityEventValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.StartAt).LessThan(x => x.EndAt);
        }
    }
}
