using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeCourseStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public CourseStatus Status { get; set; }

        public string Comment { get; set; }
    }
}
