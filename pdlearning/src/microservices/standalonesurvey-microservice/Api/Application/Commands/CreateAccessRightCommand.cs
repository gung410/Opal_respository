using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class CreateAccessRightCommand : BaseStandaloneSurveyCommand
    {
        public CreateAccessRightRequest CreationRequest { get; set; }

        public Guid UserId { get; set; }
    }
}
