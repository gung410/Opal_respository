using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    public class UserBookmark : AuditedEntity
    {
        public static readonly int MaxItemNameLength = 100;
        public static readonly int MaxCommentLength = 2000;

        public Guid UserId { get; set; }

        public BookmarkType ItemType { get; set; }

        public Guid ItemId { get; set; }

        public string ItemName { get; set; }

        public string Comment { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
