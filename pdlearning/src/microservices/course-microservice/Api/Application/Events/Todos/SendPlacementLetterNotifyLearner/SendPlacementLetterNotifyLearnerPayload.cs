using System.Collections.Generic;
using Microservice.Course.Application.Models;

namespace Microservice.Course.Application.Events.Todos
{
    public class SendPlacementLetterNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }

        public string CourseCode { get; set; }

        public List<SessionNotificationModel> CourseSessions { get; set; }
    }
}
