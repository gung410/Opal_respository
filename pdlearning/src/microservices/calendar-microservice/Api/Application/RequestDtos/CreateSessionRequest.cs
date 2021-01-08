using System;

namespace Microservice.Calendar.Application.RequestDtos
{
    public class CreateSessionRequest
    {
        public Guid SessionId { get; set; }

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
