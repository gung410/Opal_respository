namespace Microservice.Course.Application.Events.Todos
{
    public class AssessmentAnswerCompletedNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassRunTitle { get; set; }

        public string AssignmentName { get; set; }

        public string AssessorName { get; set; }
    }
}
