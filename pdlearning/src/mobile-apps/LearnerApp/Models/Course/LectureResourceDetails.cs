using LearnerApp.Models.Content;

namespace LearnerApp.Models.Course
{
    public class LectureResourceDetails
    {
        public string CourseId { get; set; }

        public string Id { get; set; }

        public string ResourceId { get; set; }

        public string Title { get; set; }

        public DigitalContentConfig DigitalContentConfig { get; set; }
    }
}
