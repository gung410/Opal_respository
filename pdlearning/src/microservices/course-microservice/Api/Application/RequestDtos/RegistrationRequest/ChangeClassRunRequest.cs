using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeClassRunRequest
    {
        public Guid RegistrationId { get; set; }

        public Guid ClassRunChangeId { get; set; }

        public string Comment { get; set; }
    }
}
