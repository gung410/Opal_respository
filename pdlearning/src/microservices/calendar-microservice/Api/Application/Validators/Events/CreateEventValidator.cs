using FluentValidation;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Timing;

namespace Microservice.Calendar.Application.Validators.Events
{
    public class CreateEventValidator : AbstractValidator<CreatePersonalEventRequest>
    {
        public CreateEventValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.StartAt).GreaterThan(Clock.Now);
            RuleFor(x => x.StartAt).LessThan(x => x.EndAt);
        }
    }
}
