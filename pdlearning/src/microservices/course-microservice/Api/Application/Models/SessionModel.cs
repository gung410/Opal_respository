using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class SessionModel
    {
        public SessionModel()
        {
        }

        public SessionModel(Session entity)
        {
            Id = entity.Id;
            ClassRunId = entity.ClassRunId;
            SessionTitle = entity.SessionTitle;
            Venue = entity.Venue;
            LearningMethod = entity.LearningMethod;
            StartDateTime = entity.StartDateTime;
            EndDateTime = entity.EndDateTime;
            RescheduleStartDateTime = entity.RescheduleStartDateTime;
            RescheduleEndDateTime = entity.RescheduleEndDateTime;
            CreatedDate = entity.CreatedDate;
            UsePreRecordClip = entity.UsePreRecordClip;
            PreRecordId = entity.PreRecordId;
            PreRecordPath = entity.PreRecordPath;
        }

        public Guid Id { get; set; }

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public string Venue { get; set; }

        public bool LearningMethod { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Optional property: Dont set it in constructor.
        /// </summary>
        public string SessionCode { get; set; }

        public bool UsePreRecordClip { get; set; }

        /// <summary>
        /// This is video path of PreRecordId from ccpm. This is a relative S3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        /// <summary>
        /// This is video content id from ccpm.
        /// </summary>
        public Guid? PreRecordId { get; set; }
    }
}
