using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class CheckExistedCourseFieldQuery : BaseThunderQuery<bool>
    {
        public string ExternalCode { get; set; }

        public string CourseCode { get; set; }

        public Guid? CourseId { get; set; }
    }
}
