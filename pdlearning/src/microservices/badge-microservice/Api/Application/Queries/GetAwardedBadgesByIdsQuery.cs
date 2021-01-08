using System;
using System.Collections.Generic;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetAwardedBadgesByIdsQuery : BaseThunderQuery<PagedResultDto<UserBadgeModel>>
    {
        public Guid UserId { get; init; }

        public List<GetAwardedBadgesByIdsDto> Data { get; init; }

        public PagedResultRequestDto PageInfo { get; init; }
    }
}
