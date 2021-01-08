using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class CreateCommentRequest
    {
        public string Content { get; set; }

        public string ObjectId { get; set; }

        public EntityCommentType EntityCommentType { get; set; }
    }
}
