using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteAccessRightCommand : BaseStandaloneSurveyCommand
    {
        public Guid Id { get; set; }
    }
}
