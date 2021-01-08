using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402
namespace Microservice.Webinar.Application.Commands
{
    public class SaveAttendeesCommand : BaseThunderCommand
    {
        public List<SaveAttendeesCommandItem> Attendees { get; set; }
    }

    public class SaveAttendeesCommandItem
    {
        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }

        public bool IsModerator { get; set; }
    }
}
#pragma warning restore SA1402
