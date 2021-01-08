using System;

namespace Microservice.Course.Application.Events.Todos
{
    public class PublishedCourseNotifyLearnerEvent : BaseTodoEvent<PublishedCourseNotifyLearnerPayload>
    {
        public PublishedCourseNotifyLearnerEvent(Guid createBy, PublishedCourseNotifyLearnerPayload payload)
        {
            TaskURI = $"urn:schemas:conexus:dls:course-api:course-published-for-learner:{Guid.NewGuid()}";
            Subject = "New Course published";
            Template = "CoursePublishedNotifyLearner";
            Payload = payload;
            CreatedBy = createBy;
        }
    }
}
