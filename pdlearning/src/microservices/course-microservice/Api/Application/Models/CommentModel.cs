using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class CommentModel
    {
        public CommentModel()
        {
        }

        public CommentModel(Comment comment)
        {
            Id = comment.Id;
            Content = comment.Content;
            UserId = comment.UserId;
            ObjectId = comment.ObjectId;
            CreatedDate = comment.CreatedDate;
            ChangedDate = comment.ChangedDate;
            Action = comment.Action;
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public string Action { get; set; }
    }
}
