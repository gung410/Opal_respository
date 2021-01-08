using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetCommentNotSeenQuery : BaseThunderQuery<IEnumerable<SeenCommentModel>>
    {
        public IEnumerable<Guid> ObjectIds { get; set; }

        public EntityCommentType EntityCommentType { get; set; }
    }
}
