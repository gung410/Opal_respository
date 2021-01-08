using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class MarkFormAsArchivedCommand : BaseStandaloneSurveyCommand
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }
    }
}
