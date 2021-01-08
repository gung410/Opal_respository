using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class ConfirmBlockoutDateRequest
    {
        public Guid CoursePlanningCycleId { get; set; }
    }
}
