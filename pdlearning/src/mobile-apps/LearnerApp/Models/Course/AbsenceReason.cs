namespace LearnerApp.Models.Course
{
    public class AbsenceReason
    {
        public string SessionId { get; set; }

        public string UserId { get; set; }

        public string Reason { get; set; }

        public string[] Attachment { get; set; }
    }
}
