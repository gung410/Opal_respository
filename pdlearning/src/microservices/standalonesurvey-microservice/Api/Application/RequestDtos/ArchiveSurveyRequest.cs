using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class ArchiveSurveyRequest : HasSubModuleInfoBase
    {
        public Guid ObjectId { get; set; }

        public Guid? ArchiveByUserId { get; set; }
    }
}
