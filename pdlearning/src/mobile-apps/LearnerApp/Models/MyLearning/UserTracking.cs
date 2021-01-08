namespace LearnerApp.Models.MyLearning
{
    public class UserTracking
    {
        public string EventName { get; set; }

        public object Payload { get; set; }

        public string Time { get; set; }

        public string SessionId { get; set; }

        public string UserId { get; set; }
    }
}
