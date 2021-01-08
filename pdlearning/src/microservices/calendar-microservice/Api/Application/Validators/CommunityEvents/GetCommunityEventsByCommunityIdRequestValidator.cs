using FluentValidation;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Validators.CommunityEvents
{
    public class GetCommunityEventsByCommunityIdRequestValidator : AbstractValidator<GetCommunityEventsByCommunityIdRequest>
    {
        public GetCommunityEventsByCommunityIdRequestValidator()
        {
            RuleFor(x => x.CalendarEventSource).NotEmpty();
            RuleFor(x => x.CommunityId).NotEmpty();
        }
    }
}
