using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveClassRunDto
    {
        public Guid? Id { get; set; }

        public Guid CourseId { get; set; }

        public string ClassTitle { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public IEnumerable<Guid> FacilitatorIds { get; set; }

        public IEnumerable<Guid> CoFacilitatorIds { get; set; }

        public int MinClassSize { get; set; }

        public int MaxClassSize { get; set; }

        public DateTime? ApplicationStartDate { get; set; }

        public DateTime? ApplicationEndDate { get; set; }
    }
}
