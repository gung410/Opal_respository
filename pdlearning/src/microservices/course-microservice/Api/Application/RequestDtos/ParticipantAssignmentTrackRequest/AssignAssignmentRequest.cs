using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class AssignAssignmentRequest
    {
        public List<AssignAssignmentDto> Registrations { get; set; }

        public Guid AssignmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
