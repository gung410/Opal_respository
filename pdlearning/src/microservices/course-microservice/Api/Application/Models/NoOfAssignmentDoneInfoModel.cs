using System;

namespace Microservice.Course.Application.Models
{
    public class NoOfAssignmentDoneInfoModel
    {
        public Guid RegistrationId { get; set; }

        public int TotalAssignments { get; set; }

        public int DoneAssignments { get; set; }
    }
}
