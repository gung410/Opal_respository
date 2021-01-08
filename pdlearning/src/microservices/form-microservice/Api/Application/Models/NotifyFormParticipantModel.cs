using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Form.Application.Models
{
    public class NotifyFormParticipantModel
    {
        public Guid ParcitipantId { get; set; }

        public string ParticipantName { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public string FormTitle { get; set; }

        public string FormOwnerName { get; set; }
    }
}
