using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class SearchAccessRightQuery : BaseThunderQuery<PagedResultDto<AccessRightModel>>
    {
        public SearchAccessRightQuery(Guid originalObjectId, PagedResultRequestDto pagedInfo)
        {
            OriginalObjectId = originalObjectId;
            PagedInfo = pagedInfo;
        }

        public Guid OriginalObjectId { get; }

        public PagedResultRequestDto PagedInfo { get; }
    }
}
