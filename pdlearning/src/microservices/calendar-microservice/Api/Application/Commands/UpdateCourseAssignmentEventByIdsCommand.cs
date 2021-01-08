using System;
using System.Collections.Generic;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateCourseAssignmentEventByIdsCommand : BaseCalendarCommand
    {
        public List<Guid> AssignmentEventIds { get; set; }

        public EventStatus Status { get; set; }
    }
}
