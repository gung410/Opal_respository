using System;
using System.Collections.Generic;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteCourseAssignmentEventByIdsCommand : BaseCalendarCommand
    {
        public List<Guid> EventIds { get; set; }
    }
}
