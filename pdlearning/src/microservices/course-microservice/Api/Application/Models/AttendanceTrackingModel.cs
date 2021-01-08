using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class AttendanceTrackingModel
    {
        public AttendanceTrackingModel(AttendanceTracking entity)
        {
            Id = entity.Id;
            SessionId = entity.SessionId;
            UserId = entity.Userid;
            ReasonForAbsence = entity.ReasonForAbsence ?? string.Empty;
            Attachment = entity.Attachment;
            Status = entity.Status;
            IsCodeScanned = entity.IsCodeScanned;
            CodeScannedDate = entity.CodeScannedDate;
        }

        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public bool IsCodeScanned { get; set; }

        public DateTime? CodeScannedDate { get; set; }

        public string ReasonForAbsence { get; set; }

        public IEnumerable<string> Attachment { get; set; }

        public AttendanceTrackingStatus? Status { get; set; } = null;
    }
}
