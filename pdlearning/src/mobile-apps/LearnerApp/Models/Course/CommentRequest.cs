using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class CommentRequest
    {
        public string ObjectId { get; set; }

        public EntityCommentType EntityCommentType { get; set; }

        public string ActionType { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
