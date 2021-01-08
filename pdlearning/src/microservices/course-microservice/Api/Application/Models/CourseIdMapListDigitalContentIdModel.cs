using System;

namespace Microservice.Course.Application.Models
{
    public class CourseIdMapListDigitalContentIdModel
    {
        public Guid CourseId { get; set; }

        public Guid[] ListDigitalContentId { get; set; }
    }
}
