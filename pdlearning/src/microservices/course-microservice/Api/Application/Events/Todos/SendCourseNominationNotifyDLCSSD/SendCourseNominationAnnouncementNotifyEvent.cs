using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendCourseNominationAnnouncementNotifyEvent : BaseTodoEvent<SendCourseNominationAnnouncementNotifyPayload>
    {
        public static string InAppMessageTemplate = @"We would like to inform you that the course - {CourseTitle} - has been published. 
            We believe that this course is very informative and will contribute to your staff's professional development. 
            Please consider and nominate the course to your staff if possible.";

        public SendCourseNominationAnnouncementNotifyEvent(
            Guid createBy,
            SendCourseNominationAnnouncementNotifyPayload payload,
            List<Guid> assignedToIds,
            string message)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-course-nomination:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Nomination Announcement";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
            Message = message;
            PlainText = GetReplacedTagsInAppMessage(payload);
        }

        public List<Guid> AssignedToIds { get; }

        private string GetReplacedTagsInAppMessage(SendCourseNominationAnnouncementNotifyPayload payload)
        {
            return InAppMessageTemplate.Replace("{CourseTitle}", payload.CourseTitle);
        }
    }
}
