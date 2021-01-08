using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetAssignmentRequest : PagedResultRequestDto
    {
        public Guid RegistrationId { get; set; }
    }
}
