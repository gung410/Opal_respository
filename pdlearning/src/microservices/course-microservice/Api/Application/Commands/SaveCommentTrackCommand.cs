using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveCommentTrackCommand : BaseThunderCommand
    {
        public IEnumerable<Guid> CommentIds { get; set; }
    }
}
