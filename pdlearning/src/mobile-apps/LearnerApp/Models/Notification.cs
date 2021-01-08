using LearnerApp.Models.OutstandingTask;

namespace LearnerApp.Models
{
    public class Notification
    {
        public OutstandingTaskTypeEnum ObjectType { get; set; }

        public string ObjectId { get; set; }
    }
}
