using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class UserBookmarkModel
    {
        public UserBookmarkModel()
        {
        }

        public UserBookmarkModel(UserBookmark userBookmark)
        {
            Id = userBookmark.Id;
            UserId = userBookmark.UserId;
            ItemType = userBookmark.ItemType;
            ItemId = userBookmark.ItemId;
            ItemName = userBookmark.ItemName;
            Comment = userBookmark.Comment;
            CreatedDate = userBookmark.CreatedDate;
            CreatedBy = userBookmark.CreatedBy;
            ChangedDate = userBookmark.ChangedDate;
            ChangedBy = userBookmark.ChangedBy;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public BookmarkType ItemType { get; set; }

        public Guid ItemId { get; set; }

        public string ItemName { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
