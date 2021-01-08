using System;
using System.Collections.Generic;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveClassRunCommand : BaseThunderCommand
    {
        public SaveClassRunCommand(SaveClassRunRequest request)
        {
            // Progress data for StartDateTime/EndDateTime
            Id = request.Data.Id ?? Guid.NewGuid();
            IsCreate = !request.Data.Id.HasValue;
            ClassTitle = request.Data.ClassTitle;
            ApplicationEndDate = request.Data.ApplicationEndDate;
            ApplicationStartDate = request.Data.ApplicationStartDate;
            FacilitatorIds = request.Data.FacilitatorIds;
            CoFacilitatorIds = request.Data.CoFacilitatorIds;
            CourseId = request.Data.CourseId;
            StartDateTime = request.Data.StartDateTime;
            EndDateTime = request.Data.EndDateTime;
            PlanningStartTime = request.Data.PlanningStartTime;
            PlanningEndTime = request.Data.PlanningEndTime;
            MaxClassSize = request.Data.MaxClassSize;
            MinClassSize = request.Data.MinClassSize;
        }

        public Guid Id { get; set; }

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

        public bool IsCreate { get; set; }
    }
}
