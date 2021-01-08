using System;
using System.Collections.Generic;
using System.Text;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveAnnouncementCommand : BaseThunderCommand
    {
        public SaveAnnouncementCommand(SaveAnnouncementDto data, Guid currentUserId)
        {
            CurrentUserId = currentUserId;
            Id = data.Id ?? Guid.NewGuid();
            IsCreate = !data.Id.HasValue;
            Title = data.Title;
            Message = Encoding.UTF8.GetString(Convert.FromBase64String(data.Base64Message));
            ScheduleDate = data.ScheduleDate;
            RegistrationIds = data.RegistrationIds;
            CourseId = data.CourseId;
            ClassrunId = data.ClassrunId;
            IsSentToAllParticipants = data.IsSentToAllParticipants;
        }

        public Guid CurrentUserId { get; set; }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public IEnumerable<Guid> RegistrationIds { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassrunId { get; set; }

        public bool IsCreate { get; set; }

        public bool IsSentToAllParticipants { get; set; }
    }
}
