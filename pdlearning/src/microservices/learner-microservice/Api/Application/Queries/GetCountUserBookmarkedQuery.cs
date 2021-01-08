using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetCountUserBookmarkedQuery : BaseThunderQuery<List<UserBookmarkedModel>>
    {
        public BookmarkType ItemType { get; set; }

        public Guid[] ItemIds { get; set; }
    }
}
