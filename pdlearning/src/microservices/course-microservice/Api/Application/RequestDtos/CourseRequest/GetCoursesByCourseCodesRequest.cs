using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetCoursesByCourseCodesRequest
    {
        public IEnumerable<string> CourseCodes { get; set; }
    }
}
