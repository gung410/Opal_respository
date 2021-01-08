namespace Microservice.Course.Application.Events.Todos
{
    public class SubmittedAssignmentNotifyCourseFacilitatorPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }

        public string ClassRunTitle { get; set; }
    }
}
