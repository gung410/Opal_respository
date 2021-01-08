using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class GetCountUserBookmarkedRequestDto
    {
        public BookmarkType ItemType { get; set; }

        public Guid[] ItemIds { get; set; }
    }
}
