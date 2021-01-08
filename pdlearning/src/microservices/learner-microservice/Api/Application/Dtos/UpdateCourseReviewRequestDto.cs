namespace Microservice.Learner.Application.Dtos
{
    public class UpdateCourseReviewRequestDto
    {
        public int Rating { get; set; }

        public string CommentContent { get; set; }
    }
}
