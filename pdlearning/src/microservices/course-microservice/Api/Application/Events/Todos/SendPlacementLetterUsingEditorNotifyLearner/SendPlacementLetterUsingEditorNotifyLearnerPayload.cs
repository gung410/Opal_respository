namespace Microservice.Course.Application.Events.Todos
{
    public class SendPlacementLetterUsingEditorNotifyLearnerPayload : BaseTodoEventPayload
    {
        public string CourseName { get; set; }

        public string CourseCode { get; set; }
    }
}
