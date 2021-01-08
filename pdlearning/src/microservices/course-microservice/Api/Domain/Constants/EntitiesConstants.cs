namespace Microservice.Course.Domain.Constants
{
    public class EntitiesConstants
    {
        // Course
        public static readonly int CourseNameLength = 2000;

        public static readonly int CourseCodeLength = 50;

        public static readonly int ExternalCodeLength = 50;

        public static readonly int CourseLevelLength = 20;

        public static readonly int CourseCourseTypeLength = 30;

        // Section
        public static readonly int SectionTitleLength = 256;

        // Lecture
        public static readonly int LectureContentTitleLength = 256;

        public static readonly int LectureMimeTypeLength = 450;

        public static readonly int LectureNameLength = 450;

        public static readonly int LectureIconLength = 20;

        public static readonly string UniqueCodeLength = "D6";

        public static readonly string UniqueClassRunNumberLength = "D2";

        public static readonly int ExternalIDLength = 512;

        // Assignment
        public static readonly int AssignmentTitleLength = 100;

        // Suffix of Full text search field must is 'FullTextSearch'. It will use for dynamic filter. Follow GetFullTextFilteredEntitiesSharedQuery
        public static readonly string FullTextSearchSuffix = "FullTextSearch";
    }
}
