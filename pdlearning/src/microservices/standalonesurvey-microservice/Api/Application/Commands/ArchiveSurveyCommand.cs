using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class ArchiveSurveyCommand : BaseStandaloneSurveyCommand
    {
        public Guid FormId { get; set; }

        public Guid? ArchiveBy { get; set; }
    }
}
