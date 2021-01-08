using System;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Domain.ValueObject;

namespace Microservice.NewsFeed.Application.Models
{
    public class CourseFeedModel
    {
        public CourseFeedModel(PdpmSuggestCourseFeed courseFeed)
        {
            Id = courseFeed.Id;
            CourseId = courseFeed.CourseId;
            UserId = courseFeed.UserId;
            CreatedDate = courseFeed.CreatedDate;
            ThumbnailUrl = courseFeed.ThumbnailUrl;
            CourseName = courseFeed.CourseName;
            UpdateInfo = courseFeed.UpdateInfo;
            Url = courseFeed.Url;
        }

        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ThumbnailUrl { get; set; }

        public string CourseName { get; set; }

        public UpdateCourseInfoType UpdateInfo { get; set; }

        public string Url { get; set; }

        public string Type => nameof(PdpmSuggestCourseFeed);
    }
}
