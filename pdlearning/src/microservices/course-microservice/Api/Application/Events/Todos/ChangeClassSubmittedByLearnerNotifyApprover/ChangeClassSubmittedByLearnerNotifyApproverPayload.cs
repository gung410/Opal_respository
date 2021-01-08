namespace Microservice.Course.Application.Events.Todos
{
    public class ChangeClassSubmittedByLearnerNotifyApproverPayload : BaseTodoEventPayload
    {
        public string LearnerName { get; set; }

        public string CourseTitle { get; set; }
    }
}
