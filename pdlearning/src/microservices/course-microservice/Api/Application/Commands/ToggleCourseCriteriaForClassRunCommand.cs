using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ToggleCourseCriteriaForClassRunCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool CourseCriteriaActivated { get; set; }
    }
}
