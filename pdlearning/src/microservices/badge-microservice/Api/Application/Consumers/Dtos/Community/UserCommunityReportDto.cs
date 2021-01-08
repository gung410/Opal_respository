using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class UserCommunityReportDto
    {
        public Guid UserId { get; set; }

        public Guid CommunityId { get; set; }

        public IEnumerable<CommunityPostResponses> Posts { get; set; }

        public CommunityStatistic ToCommunityStatistic()
        {
            CommunityStatistic statistic = new();
            return statistic;
        }
    }
}
