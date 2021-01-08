using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CompleteLectureInMyCourseCommand : BaseThunderCommand
    {
        public Guid LectureInMyCourseId { get; set; }
    }
}
