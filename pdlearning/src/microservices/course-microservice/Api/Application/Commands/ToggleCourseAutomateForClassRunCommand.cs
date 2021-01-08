using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ToggleCourseAutomateForClassRunCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool CourseAutomateActivated { get; set; }
    }
}
