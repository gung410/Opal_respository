using System.Collections.Generic;

namespace LearnerApp.Models.Learner
{
    public class EditReview
    {
        public List<UserReview> UserReviews { get; set; }

        public bool IsMicrolearningType { get; set; }
    }
}
