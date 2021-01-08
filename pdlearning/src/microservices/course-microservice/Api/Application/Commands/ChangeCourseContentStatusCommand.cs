using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeCourseContentStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public ContentStatus ContentStatus { get; set; }
    }
}
