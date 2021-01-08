namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerAssignedNotifyAssigneePayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassRunTitle { get; set; }

        public string AssignmentName { get; set; }

        public string AssignorName { get; set; }

        public string LearnerName { get; set; }
    }
}
