using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendPlacementLetterUsingEditorNotifyLearnerEvent : BaseTodoEvent<SendPlacementLetterUsingEditorNotifyLearnerPayload>
    {
        public static string InAppMessageTemplate = @"You have enrolled successfully in the PD Activity: {CourseTitle} - {CourseCode}.";

        public SendPlacementLetterUsingEditorNotifyLearnerEvent(Guid createBy, SendPlacementLetterUsingEditorNotifyLearnerPayload payload, List<Guid> assignedToIds, string message)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:send-placement-lettet-editor:{Guid.NewGuid()}";
            Subject = "OPAL2.0 - Placement Letter";
            Payload = payload;
            AssignedToIds = assignedToIds;
            CreatedBy = createBy;
            Message = message;
            PlainText = GetReplacedTagsInAppMessage(payload);
        }

        public List<Guid> AssignedToIds { get; }

        private string GetReplacedTagsInAppMessage(SendPlacementLetterUsingEditorNotifyLearnerPayload payload)
        {
            var message = InAppMessageTemplate.Replace("{CourseTitle}", payload.CourseName);

            return message.Replace("{CourseCode}", payload.CourseCode);
        }
    }
}
