namespace Microservice.Course.Application.Events.Todos
{
    public class ApprovedCourseMLUNotifyCollaboratorPayload : BaseTodoEventPayload
    {
        public string CourseTitle { get; set; }

        public string CoursePDArea { get; set; }
    }
}
