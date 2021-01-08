using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class AssignAssignmentDto
    {
        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public AssignAssignmentCommandRegistration ToAssignAssignmentCommandRegistrationCommand()
        {
            return new AssignAssignmentCommandRegistration
            {
                RegistrationId = RegistrationId,
                UserId = UserId
            };
        }
    }
}
