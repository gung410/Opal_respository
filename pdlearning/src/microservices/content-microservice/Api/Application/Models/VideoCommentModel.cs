using System;
using Microservice.Content.Domain.ValueObject;
using VideoCommentEntity = Microservice.Content.Domain.Entities.VideoComment;

namespace Microservice.Content.Application.Models
{
    public class VideoCommentModel
    {
        public VideoCommentModel()
        {
        }

        public VideoCommentModel(VideoCommentEntity comment)
        {
            Id = comment.Id;
            UserId = comment.UserId;
            ObjectId = comment.ObjectId ?? Guid.Empty;
            OriginalObjectId = comment.OriginalObjectId ?? Guid.Empty;
            SourceType = comment.SourceType;
            Content = comment.Content;
            VideoId = comment.VideoId;
            VideoTime = comment.VideoTime;
            Note = comment.Note;
            CreatedDate = comment.CreatedDate;
            ChangedDate = comment.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ObjectId { get; set; }

        public VideoSourceType SourceType { get; set; }

        public Guid OriginalObjectId { get; set; }

        public string Content { get; set; }

        public Guid VideoId { get; set; }

        public int VideoTime { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
