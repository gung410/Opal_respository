using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveAnnouncementDto
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string Base64Message { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public IEnumerable<Guid> RegistrationIds { get; set; } = new List<Guid>();

        public Guid CourseId { get; set; }

        public Guid ClassrunId { get; set; }

        public bool SaveTemplate { get; set; }

        public bool IsSentToAllParticipants { get; set; }
    }
}
