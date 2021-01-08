namespace Microservice.Course.Application.Events.Todos
{
    public class TakeAttendanceNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseFacilitatorName { get; set; }

        public string CourseFacilitatorEmail { get; set; }

        public string CourseAdminName { get; set; }

        public string CourseAdminEmail { get; set; }

        public string SessionTitle { get; set; }

        public string ClassrunTitle { get; set; }
    }
}
