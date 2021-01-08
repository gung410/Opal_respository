using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.LnaForm.Application.Services.FormParticipant.Dtos
{
    public class AssignFormParticipantsDto
    {
        public List<Guid> UserIds { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }
    }
}
