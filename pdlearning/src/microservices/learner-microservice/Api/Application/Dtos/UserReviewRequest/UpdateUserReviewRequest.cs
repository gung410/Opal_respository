using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class UpdateUserReviewRequest
    {
        public ItemType ItemType { get; set; }

        public int Rating { get; set; }

        public string CommentContent { get; set; }
    }
}
