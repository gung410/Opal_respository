using System.Collections.Generic;
using Microservice.Badge.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetBadgesQuery : BaseThunderQuery<List<BadgeModel>>
    {
    }
}
