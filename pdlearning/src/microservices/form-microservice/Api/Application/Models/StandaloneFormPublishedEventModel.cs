using System;
using System.Collections.Generic;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Models
{
    public class StandaloneFormPublishedEventModel
    {
        public StandaloneFormPublishedEventModel()
        {
        }

        public StandaloneFormPublishedEventModel(FormEntity form, List<Guid> participantIds, string formUrl)
        {
            Form = form;
            ParticipantIds = participantIds;
            FormUrl = formUrl;
        }

        public FormEntity Form { get; set; }

        public string FormUrl { get; set; }

        public List<Guid> ParticipantIds { get; set; }
    }
}
