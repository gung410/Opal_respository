using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ExportParticipantsRequest
    {
        public Guid CourseId { get; set; }

        public List<Guid> ClassRunIds { get; set; }

        public ExportParticipantsFileFormat FileFormat { get; set; }
    }
}
