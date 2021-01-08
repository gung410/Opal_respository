using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetCommentNotSeenRequest
    {
        public IEnumerable<Guid> ObjectIds { get; set; }

        public EntityCommentType EntityCommentType { get; set; }
    }
}
