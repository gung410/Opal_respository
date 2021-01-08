using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class CheckExistedSessionFieldQuery : BaseThunderQuery<bool>
    {
        public DateTime SessionDate { get; set; }

        public Guid? SessionId { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
