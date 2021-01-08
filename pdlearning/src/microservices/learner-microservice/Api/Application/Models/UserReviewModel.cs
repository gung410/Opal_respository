using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class UserReviewModel
    {
        public UserReviewModel()
        {
        }

        public UserReviewModel(UserReview entity)
        {
            Id = entity.Id;
            ParentCommentId = entity.ParentCommentId;
            UserId = entity.UserId;
            ItemId = entity.ItemId;
            ItemType = entity.ItemType;
            Version = entity.Version;
            ItemName = entity.ItemName;
            UserFullName = entity.UserFullName;
            CommentTitle = entity.CommentTitle;
            CommentContent = entity.CommentContent;
            Rate = entity.Rate;
            IsDeleted = entity.IsDeleted;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            ChangedBy = entity.ChangedBy;
            ChangedDate = entity.ChangedDate;
            ClassRunId = entity.ClassRunId;
        }

        public Guid Id { get; set; }

        public Guid? ParentCommentId { get; set; }

        public Guid UserId { get; set; }

        public Guid ItemId { get; set; }

        public ItemType ItemType { get; set; }

        public string Version { get; set; }

        public string ItemName { get; set; }

        public string UserFullName { get; set; }

        public string CommentTitle { get; set; }

        public string CommentContent { get; set; }

        public double? Rate { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
