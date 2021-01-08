using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class SearchCommentRequest : HasSubModuleInfoBase
    {
        public Guid ObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
