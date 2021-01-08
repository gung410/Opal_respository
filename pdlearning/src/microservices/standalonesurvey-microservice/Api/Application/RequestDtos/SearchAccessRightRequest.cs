using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class SearchAccessRightRequest : HasSubModuleInfoBase
    {
        public Guid OriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
