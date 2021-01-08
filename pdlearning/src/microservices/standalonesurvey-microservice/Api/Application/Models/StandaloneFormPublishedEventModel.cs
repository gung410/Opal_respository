using System;
using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class StandaloneFormPublishedEventModel
    {
        public StandaloneFormPublishedEventModel()
        {
        }

        public StandaloneFormPublishedEventModel(Domain.Entities.StandaloneSurvey form, List<Guid> participantIds, string formUrl)
        {
            Form = form;
            ParticipantIds = participantIds;
            FormUrl = formUrl;
        }

        public Domain.Entities.StandaloneSurvey Form { get; set; }

        public string FormUrl { get; set; }

        public List<Guid> ParticipantIds { get; set; }
    }
}
