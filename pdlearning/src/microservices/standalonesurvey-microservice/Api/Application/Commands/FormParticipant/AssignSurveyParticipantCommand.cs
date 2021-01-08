using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class AssignSurveyParticipantCommand : BaseStandaloneSurveyCommand
    {
        public List<Guid> UserIds { get; set; }

        public Guid SurveyOriginalObjectId { get; set; }

        public Guid SurveyId { get; set; }

        public bool? IsStarted { get; set; }

        public SurveyParticipantStatus? Status { get; set; }

        public Guid CurrentUserId { get; set; }
    }
}
