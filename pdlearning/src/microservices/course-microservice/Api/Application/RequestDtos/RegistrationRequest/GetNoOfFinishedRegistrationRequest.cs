using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetNoOfFinishedRegistrationRequest
    {
        public Guid CourseId { get; set; }

        public int DepartmentId { get; set; }

        public DateTime? ForClassRunEndAfter { get; set; }

        public DateTime? ForClassRunEndBefore { get; set; }
    }
}
