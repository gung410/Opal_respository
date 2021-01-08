using System;
using System.Collections.Generic;
using LearnerApp.Common;

namespace LearnerApp.Models
{
    public class CourseInformation
    {
        public string Id { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string CourseType { get; set; }

        public string CourseContent { get; set; }

        public string CourseLevel { get; set; }

        public string Description { get; set; }

        public int DurationMinutes { get; set; }

        public string Status { get; set; }

        public List<string> Tags { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string ThumbnailUrl { get; set; }

        public string PdActivityType { get; set; }

        public bool IsCourseExpired()
        {
            return ExpiredDate.HasValue && DateTime.Compare(ExpiredDate.Value, DateTime.Now) < 0;
        }

        public bool IsMicrolearningType()
        {
            return !string.IsNullOrEmpty(PdActivityType) && PdActivityType.Equals(MetadataId.Microlearning);
        }
    }
}
