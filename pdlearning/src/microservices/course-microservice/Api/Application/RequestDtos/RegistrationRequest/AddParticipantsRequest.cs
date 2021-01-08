using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class AddParticipantsRequest
    {
        public Guid ClassRunId { get; set; }

        public List<Guid> UserIds { get; set; }

        public bool FollowCourseTargetParticipant { get; set; }

        public List<int> DepartmentIds { get; set; }
    }
}
