using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityHierarchyTreeQuery : BaseThunderQuery<List<CommunityModel>>
    {
        public Guid UserId { get; set; }
    }
}
