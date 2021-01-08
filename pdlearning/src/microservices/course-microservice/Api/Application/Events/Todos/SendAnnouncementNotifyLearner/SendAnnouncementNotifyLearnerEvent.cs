using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendAnnouncementNotifyLearnerEvent : BaseTodoEvent<SendAnnouncementNotifyLearnerPayload>
    {
        public static readonly string AnnouncementTemplate =
            @"<p>Dear #USER_NAME#,</p>
             <p></p>
             <p> You have one new announcement from your #COURSE_TITLE#, #CLASSRUNTITLE_TITLE#.</p>
             <p></p>
             <p>#ANNOUNCEMENTCONTENT_CONTENT#</p>
             <p></p>
             <p>This is a system-generated notification.Please do not reply.</p>
             <p></p>
             <p>Thank you.</p>
             <p></p>
             <p>Best regards,</p>
             <p>The OPAL2.0 team</p>";

        public static string InAppMessageTemplate = @"You have one new course announcement from your {CourseTitle} - {ClassTitle}. 
            Please check your email for your information";

        public SendAnnouncementNotifyLearnerEvent(Guid createBy, SendAnnouncementNotifyLearnerPayload payload, List<Guid> assignedToIds)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-announcement:{Guid.NewGuid()}";
            Subject = $"OPAL2.0 - New Course Announcement - {payload.AnnouncementTitle}";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
            Message = GetReplacedTagsMessage(AnnouncementTemplate, payload);
            PlainText = GetReplacedTagsInAppMessage(payload);
        }

        public List<Guid> AssignedToIds { get; }

        private string GetReplacedTagsMessage(string message, SendAnnouncementNotifyLearnerPayload payload)
        {
            if (payload.AnnouncementContent == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(payload.UserName))
            {
                message = message.Replace("#USER_NAME#", payload.UserName);
            }

            if (!string.IsNullOrEmpty(payload.CourseTitle))
            {
                message = message.Replace("#COURSE_TITLE#", payload.CourseTitle);
            }

            if (!string.IsNullOrEmpty(payload.ClassTitle))
            {
                message = message.Replace("#CLASSRUNTITLE_TITLE#", payload.ClassTitle);
            }

            if (!string.IsNullOrEmpty(payload.AnnouncementContent))
            {
                message = message.Replace("#ANNOUNCEMENTCONTENT_CONTENT#", payload.AnnouncementContent);
            }

            return message;
        }

        private string GetReplacedTagsInAppMessage(SendAnnouncementNotifyLearnerPayload payload)
        {
            return InAppMessageTemplate
                .Replace("{CourseTitle}", payload.CourseTitle)
                .Replace("{ClassTitle}", payload.ClassTitle);
        }
    }
}
