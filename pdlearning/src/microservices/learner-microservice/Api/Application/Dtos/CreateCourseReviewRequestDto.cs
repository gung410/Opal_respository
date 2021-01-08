using System;

namespace Microservice.Learner.Application.Dtos
{
    public class CreateCourseReviewRequestDto
    {
        public Guid CourseId { get; set; }

        public int Rating { get; set; }

        public string CommentContent { get; set; }
    }
}
