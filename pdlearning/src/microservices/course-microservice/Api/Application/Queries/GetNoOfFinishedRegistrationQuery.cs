using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetNoOfFinishedRegistrationQuery : BaseThunderQuery<NoOfFinishedRegistrationModel>
    {
        public Guid CourseId { get; set; }

        public int DepartmentId { get; set; }

        public DateTime? ForClassRunEndAfter { get; set; }

        public DateTime? ForClassRunEndBefore { get; set; }
    }
}
