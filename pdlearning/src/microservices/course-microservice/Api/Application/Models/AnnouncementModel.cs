using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class AnnouncementModel
    {
        public AnnouncementModel()
        {
        }

        public AnnouncementModel(Announcement entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Message = entity.Message;
            ScheduleDate = entity.ScheduleDate;
            SentDate = entity.SentDate;
            Participants = entity.Participants;
            Status = entity.Status;
            CourseId = entity.CourseId;
            ClassrunId = entity.ClassrunId;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// Participant registration ids.
        /// </summary>
        public IEnumerable<Guid> Participants { get; set; } = new List<Guid>();

        public AnnouncementStatus Status { get; set; } = AnnouncementStatus.Scheduled;

        public Guid CourseId { get; set; }

        public Guid ClassrunId { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
