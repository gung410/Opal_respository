using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeRegistrationCourseCriteriaOverridedStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public bool CourseCriteriaOverrided { get; set; }
    }
}
