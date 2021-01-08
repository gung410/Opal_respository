using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteFormParticipantCommand : BaseStandaloneSurveyCommand
    {
        public Guid[] Ids { get; set; }

        public Guid FormId { get; set; }
    }
}
