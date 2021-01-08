using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class CheckCourseEndDateValidWithClassEndDateQuery : BaseThunderQuery<bool>
    {
        public Guid CourseId { get; set; }

        public DateTime EndDate { get; set; }
    }
}
