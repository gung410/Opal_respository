using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Domain.ValueObjects.Form;

namespace Microservice.Form.Application.Services.FormParticipant.Dtos
{
    public class UpdateFormParticipantStatusDto
    {
        public Guid FormId { get; set; }

        public FormParticipantStatus? Status { get; set; }
    }
}
