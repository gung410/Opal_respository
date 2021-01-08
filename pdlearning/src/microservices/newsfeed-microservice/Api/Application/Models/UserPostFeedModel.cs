using System;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Domain.ValueObject;

namespace Microservice.NewsFeed.Application.Models
{
    public class UserPostFeedModel
    {
        public UserPostFeedModel(UserPostFeed userPostFeed)
        {
            Id = userPostFeed.Id;
            UserId = userPostFeed.UserId;
            PostId = userPostFeed.PostId;
            PostContent = userPostFeed.PostContent;
            PostedBy = userPostFeed.PostedBy;
            CreatedDate = userPostFeed.CreatedDate;
            ChangedDate = userPostFeed.ChangedDate;
            Url = userPostFeed.Url;
            PostType = userPostFeed.PostType;
            Type = userPostFeed.Type;
            PostForward = userPostFeed.PostForward;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public int PostId { get; set; }

        public string PostContent { get; set; }

        public Guid PostedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public string Url { get; set; }

        public PostType PostType { get; set; }

        public object PostForward { get; set; }

        public string Type { get; set; }
    }
}
