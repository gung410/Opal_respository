using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeCourseStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public CourseStatus Status { get; set; }

        public string Comment { get; set; }
    }
}
