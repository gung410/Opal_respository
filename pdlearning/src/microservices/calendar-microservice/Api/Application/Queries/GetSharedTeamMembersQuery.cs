using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetSharedTeamMembersQuery : BaseThunderQuery<List<TeamMemberModel>>
    {
        public Guid AccessShareId { get; set; }

        public Guid UserId { get; set; }
    }
}
