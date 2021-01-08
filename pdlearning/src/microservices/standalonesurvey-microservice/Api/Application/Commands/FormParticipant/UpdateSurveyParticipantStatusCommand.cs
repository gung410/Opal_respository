using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class UpdateSurveyParticipantStatusCommand : BaseStandaloneSurveyCommand
    {
        public Guid FormId { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
