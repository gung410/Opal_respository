using System;
using Microservice.NewsFeed.Domain.ValueObject;

namespace Microservice.NewsFeed.Domain.Entities
{
    public class PdpmSuggestCourseFeed : NewsFeed
    {
        /// <summary>
        /// The identifier course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Thumbnail of the course.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Name of the course.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Date of course info or content course change.
        /// </summary>
        public DateTime ChangedDate { get; set; }

        /// <summary>
        /// Include <see cref="UpdateCourseInfoType"/> types.
        /// </summary>
        public UpdateCourseInfoType UpdateInfo { get; set; }

        /// <summary>
        /// Course detail url.
        /// </summary>
        public string Url { get; set; }

        public override string Type => nameof(PdpmSuggestCourseFeed);
    }
}
