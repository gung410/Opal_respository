using FluentValidation;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Timing;

namespace Microservice.Calendar.Application.Validators.Events
{
    public class UpdateEventValidator : AbstractValidator<UpdatePersonalEventRequest>
    {
        public UpdateEventValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.StartAt).LessThan(x => x.EndAt);
            RuleFor(x => x.StartAt).GreaterThan(Clock.Now);
        }
    }
}
