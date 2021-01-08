namespace Microservice.Course.Application.Events.Todos
{
    public class AssignmentDueDateNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string ClassRunTitle { get; set; }

        public string DueDate { get; set; }

        public string DaysBeforeAssignmentDeadline { get; set; }

        public string AssignmentTile { get; set; }
    }
}
