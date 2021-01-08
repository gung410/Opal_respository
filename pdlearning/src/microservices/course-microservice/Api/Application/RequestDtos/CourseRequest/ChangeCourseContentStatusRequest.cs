using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeCourseContentStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public string Comment { get; set; }
    }
}
