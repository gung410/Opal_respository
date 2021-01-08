using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendCoursePublicityNotifyLearnerEvent : BaseTodoEvent<SendCoursePublicityNotifyLearnerPayload>
    {
        public static string InAppMessageTemplate = @"We would like to inform you that the course - {CourseTitle} - has been published. 
            We find this course suitable for you, given your profile.
            Please take a look and consider whether it can contribute to your professional development.";

        public SendCoursePublicityNotifyLearnerEvent(
            Guid createBy,
            SendCoursePublicityNotifyLearnerPayload payload,
            List<Guid> assignedToIds,
            string message)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-course-publicity:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Course Publicity";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
            Message = message;
            PlainText = GetReplacedTagsInAppMessage(payload);
        }

        public List<Guid> AssignedToIds { get; }

        private string GetReplacedTagsInAppMessage(SendCoursePublicityNotifyLearnerPayload payload)
        {
            return InAppMessageTemplate.Replace("{CourseTitle}", payload.CourseTitle);
        }
    }
}
