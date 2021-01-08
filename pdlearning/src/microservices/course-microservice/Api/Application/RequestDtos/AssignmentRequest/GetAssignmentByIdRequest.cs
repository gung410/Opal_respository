using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAssignmentByIdRequest
    {
        public Guid Id { get; set; }

        public bool ForLearnerAnswer { get; set; }
    }
}
