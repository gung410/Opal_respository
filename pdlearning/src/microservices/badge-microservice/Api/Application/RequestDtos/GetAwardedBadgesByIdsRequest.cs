using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Badge.Application.RequestDtos
{
    public class GetAwardedBadgesByIdsRequest : PagedResultRequestDto
    {
       public List<GetAwardedBadgesByIdsDto> Data { get; set; }
    }

    public class GetAwardedBadgesByIdsDto
    {
         public Guid BadgeId { get; set; }

         public Guid? CommunityId { get; set; }
    }
}
