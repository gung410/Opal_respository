using System;
using System.Collections.Generic;
using Microservice.Badge.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetGeneralBadgesByUserIdQuery : BaseThunderQuery<List<UserBadgeModel>>
    {
        public GetGeneralBadgesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; init; }
    }
}
