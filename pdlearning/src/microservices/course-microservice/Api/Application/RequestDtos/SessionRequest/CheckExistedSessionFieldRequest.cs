using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CheckExistedSessionFieldRequest
    {
        public DateTime SessionDate { get; set; }

        public Guid? SessionId { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
