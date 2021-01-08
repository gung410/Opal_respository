using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent
{
    public class WebinarMeetingRequest
    {
        public Guid SessionId { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<WebinarMeetingAttendeeInfoRequest> Attendees { get; set; }

        public WebinarMeetingBookingSource Source { get; set; }

        /// <summary>
        /// This is video path of PreRecordId from ccpm. This is a relative S3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        /// <summary>
        /// This is video content id from ccpm.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public static WebinarMeetingRequest Create(Session session, List<WebinarMeetingAttendeeInfoRequest> attendees)
        {
            return new WebinarMeetingRequest()
            {
                SessionId = session.Id,
                Title = session.SessionTitle,
                StartTime = session.StartDateTime.GetValueOrDefault(),
                EndTime = session.EndDateTime.GetValueOrDefault(),
                Attendees = attendees,
                Source = WebinarMeetingBookingSource.Course,
                PreRecordId = session.UsePreRecordClip ? session.PreRecordId : null,
                PreRecordPath = session.UsePreRecordClip ? session.PreRecordPath : null
            };
        }
    }
}
