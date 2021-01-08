using System;
using CommentEntity = Microservice.Content.Domain.Entities.Comment;

namespace Microservice.Content.Application.Models
{
    public class CommentModel
    {
        public CommentModel()
        {
        }

        public CommentModel(CommentEntity comment)
        {
            Id = comment.Id;
            Content = comment.Content;
            UserId = comment.UserId;
            ObjectId = comment.ObjectId;
            CreatedDate = comment.CreatedDate;
            ChangedDate = comment.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
