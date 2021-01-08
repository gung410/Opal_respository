using LearnerApp.Common;

namespace LearnerApp.Models.Learner
{
    public class FeedbackDataTransfer
    {
        public string ContentId { get; set; }

        public string ContentTitle { get; set; }

        public UserReview OwnReview { get; set; }

        public PdActivityType ItemType { get; set; }

        public bool IsMicrolearningType { get; set; }

        public bool HasContentChanged { get; set; }
    }
}
