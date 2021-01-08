namespace LearnerApp.Models.Course
{
    public class PagedResultRequestDto
    {
        public int SkipCount { get; set; } = 0;

        public int MaxResultCount { get; set; } = 10;
    }
}
