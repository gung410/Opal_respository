using System.Collections.Generic;

namespace LearnerApp.Models.Learner
{
    public class LearningContentTransfer
    {
        public UserReview OwnReview { get; set; }

        public List<TableOfContent> Lectures { get; set; }

        public string CourseId { get; set; }

        public string MyCourseId { get; set; }

        public bool IsCourseCompleted { get; set; }

        public string ThumbnailUrl { get; set; }

        public string CourseName { get; set; }

        public int LectureIndex { get; set; }

        public bool HasContentChanged { get; set; }

        public string ClassRunId { get; set; }
    }
}
