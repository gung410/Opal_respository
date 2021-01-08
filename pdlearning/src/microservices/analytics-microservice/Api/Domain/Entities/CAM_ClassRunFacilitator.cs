using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_ClassRunFacilitator
    {
        public Guid ClassRunId { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string Type { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? DepartmentId { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }
    }
}
