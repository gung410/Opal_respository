using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402
namespace Microservice.Webinar.Application.Commands
{
    public class UpdateAttendeesCommand : BaseThunderCommand
    {
        public Guid MeetingId { get; set; }

        public List<UpdateAttendeeCommandItem> Attendees { get; set; }
    }

    public class UpdateAttendeeCommandItem
    {
        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }

        public bool IsModerator { get; set; }
    }
}
#pragma warning disable SA1402
