using System;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos
{
    public class RescheduleSessionDto
    {
        public Guid? Id { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public ChangeClassRunRescheduleStatusCommandSession ToChangeClassRunRescheduleStatusCommandSession()
        {
            return new ChangeClassRunRescheduleStatusCommandSession
            {
                Id = Id,
                EndDateTime = EndDateTime,
                StartDateTime = StartDateTime
            };
        }
    }
}
