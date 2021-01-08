using System;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Versioning.Application.RequestDtos
{
    public class GetVersionTrackingByObjectIdRequest : HasSubModuleInfoBase
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
