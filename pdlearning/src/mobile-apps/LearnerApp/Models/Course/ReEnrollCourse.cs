using System.Collections.Generic;
using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class ReEnrollCourse
    {
        public string CourseId { get; set; }

        public List<string> LectureIds { get; set; }

        public LearningCourseType CourseType { get; set; }
    }
}
