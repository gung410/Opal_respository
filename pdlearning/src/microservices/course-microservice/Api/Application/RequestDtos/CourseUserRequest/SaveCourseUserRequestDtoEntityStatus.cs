using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseUserRequestDtoEntityStatus
    {
        public bool ExternallyMastered { get; set; }

        public CourseUserStatus Status { get; set; }
    }
}
