using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class CreateMyBookmarkRequest
    {
        public Guid ItemId { get; set; }

        public BookmarkType ItemType { get; set; }
    }
}
