using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetCoursesByCourseCodesQuery : BaseThunderQuery<IEnumerable<CourseModel>>
    {
        public IEnumerable<string> CourseCodes { get; set; }
    }
}
