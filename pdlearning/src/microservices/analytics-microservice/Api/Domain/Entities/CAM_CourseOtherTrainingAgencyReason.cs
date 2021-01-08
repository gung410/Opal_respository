using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseOtherTrainingAgencyReason
    {
        public Guid CourseId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
