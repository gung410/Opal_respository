using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseUserRequestDtoIdentity
    {
        public int Id { get; set; }

        public Guid? ExtId { get; set; }
    }
}
